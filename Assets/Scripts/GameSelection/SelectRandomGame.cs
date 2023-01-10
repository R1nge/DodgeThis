using Shared;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameSelection
{
    public class SelectRandomGame : NetworkBehaviour
    {
        private bool _sceneloaded;

        private void Awake()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += Disconnect;
            if (NetworkManager.Singleton.SceneManager == null)
            {
                GameSelectionSingleton.Instance.ResetSelectedGames();
                Disconnect(0);
            }
            else
            {
                NetworkManager.Singleton.SceneManager.OnLoadComplete += SceneManagerOnOnLoadComplete;
            }
            
        }

        public override void OnNetworkSpawn()
        {
            var games = GameSelectionSingleton.Instance.GetSelectedGames();
            if (games.TrueForAll(x => x.HasBeenPlayed) || games.Count == 0 || !NetworkManager.Singleton)
            {
                GameSelectionSingleton.Instance.ResetSelectedGames();
                Disconnect(0);
            }
        }

        private void SceneManagerOnOnLoadComplete(ulong clientid, string scenename, LoadSceneMode loadscenemode)
        {
            if (!IsServer) return;
            if (_sceneloaded) return;
            SelectGame();
        }

        private void SelectGame()
        {
            _sceneloaded = true;
            var games = GameSelectionSingleton.Instance.GetSelectedGames();
            if (games.Count == 0) return;
            if (games.TrueForAll(x => x.HasBeenPlayed))
            {
                return;
            }
            
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

        private void Disconnect(ulong _)
        {
            var games = GameSelectionSingleton.Instance.GetSelectedGames();
            if (games.TrueForAll(x => x.HasBeenPlayed) || games.Count == 0 || !NetworkManager.Singleton)
            {
                NetworkManager.Singleton.Shutdown();
                GameSelectionSingleton.Instance.ResetSelectedGames();
                LobbySingleton.Instance.ResetPlayerList();
                SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnClientDisconnectCallback -= Disconnect;
            }
        }
    }
}