using Shared;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameSelection
{
    public class SelectRandomGame : NetworkBehaviour
    {
        private bool _sceneLoaded;

        private void Awake()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += Disconnect;
            if (NetworkManager.Singleton.SceneManager == null) return;
            NetworkManager.Singleton.SceneManager.OnLoadComplete += SceneManagerOnOnLoadComplete;
        }

        private void SceneManagerOnOnLoadComplete(ulong clientid, string scenename, LoadSceneMode loadscenemode)
        {
            if (!IsServer) return;
            if (_sceneLoaded) return;
            DisconnectServerRpc(0);
            SelectGame();
        }

        private void SelectGame()
        {
            _sceneLoaded = true;
            var games = GameSelectionSingleton.Instance.GetSelectedGames();
            if (games.Count == 0) return;
            DisconnectServerRpc(0);

            var selectedGame = Random.Range(0, games.Count);

            if (games[selectedGame].HasBeenPlayed)
            {
                SelectGame();
                return;
            }

            GameSelectionSingleton.Instance.GetSelectedGames()[selectedGame] = new Games
            {
                IsSelected = games[selectedGame].IsSelected,
                HasBeenPlayed = true,
                SceneName = games[selectedGame].SceneName
            };

            NetworkManager.Singleton.SceneManager.LoadScene(games[selectedGame].SceneName, LoadSceneMode.Single);
        }

        [ServerRpc(RequireOwnership = false)]
        private void DisconnectServerRpc(ulong _)
        {
            var games = GameSelectionSingleton.Instance.GetSelectedGames();
            if (games.TrueForAll(x => x.HasBeenPlayed) || games.Count == 0)
            {
                GameSelectionSingleton.Instance.ResetSelectedGames();
                LobbySingleton.Instance.ResetPlayerList();
                DisconnectClientRpc();
            }
        }

        [ClientRpc]
        private void DisconnectClientRpc()
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            NetworkManager.Singleton.Shutdown();
        }

        private void Disconnect(ulong _)
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            NetworkManager.Singleton.Shutdown();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= DisconnectServerRpc;
            }
        }
    }
}