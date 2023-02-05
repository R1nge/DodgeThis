using System.Collections;
using Shared;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameSelection
{
    public class SelectRandomGame : NetworkBehaviour
    {
        private bool _sceneLoaded;
        private SelectRandomGameUI _gameUI;

        private void Awake()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += Disconnect;
            if (NetworkManager.Singleton.SceneManager == null) return;
            NetworkManager.Singleton.SceneManager.OnLoadComplete += SceneManagerOnOnLoadComplete;
            _gameUI = GetComponent<SelectRandomGameUI>();
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
            if (games.Count == 0)
            {
                //TODO: load end game scene, where players can have a little fun before starting a new game
                StartCoroutine(Disconnect_c());
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
                Title = games[selectedGame].Title,
                IsSelected = games[selectedGame].IsSelected,
                HasBeenPlayed = true,
                SceneName = games[selectedGame].SceneName
            };

            var gameUI = GameSelectionSingleton.Instance.GetGamesUI();
            for (int i = 0; i < gameUI.Length; i++)
            {
                if (games[selectedGame].Title == gameUI[i].title)
                {
                    _gameUI.UpdateUI(gameUI[i]);
                }
            }
            
            ReviveAllPlayers();

            StartCoroutine(ChangeLevel_c(games[selectedGame].SceneName));
        }

        private void ReviveAllPlayers()
        {
            LobbySingleton.Instance.ReviveAllPlayers();
        }

        private IEnumerator ChangeLevel_c(string level)
        {
            yield return new WaitForSeconds(10);
            NetworkManager.Singleton.SceneManager.LoadScene(level, LoadSceneMode.Single);
        }

        private IEnumerator Disconnect_c()
        {
            yield return new WaitForSeconds(10);
            DisconnectServerRpc(0);
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
            GameSelectionSingleton.Instance.ResetSelectedGames();
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
            NetworkManager.Singleton.Shutdown();
        }

        private void Disconnect(ulong _)
        {
            GameSelectionSingleton.Instance.ResetSelectedGames();
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