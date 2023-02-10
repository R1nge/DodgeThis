using System;
using System.Collections;
using Shared;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace GameSelection
{
    public class SelectRandomGame : NetworkBehaviour
    {
        private bool _sceneLoaded;
        public event Action<int> OnGameSelectedEvent;

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
            SelectGame();
        }

        private void SelectGame()
        {
            _sceneLoaded = true;
            var games = GameSelectionSingleton.Instance.GetSelectedGames();
            
            if(TryLoadEndGame()) return;

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
                    OnGameSelectedEvent?.Invoke(i);
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

        private bool TryLoadEndGame()
        {
            var games = GameSelectionSingleton.Instance.GetSelectedGames();
            if (games.TrueForAll(x => x.HasBeenPlayed) || games.Count == 0)
            {
                NetworkManager.Singleton.SceneManager.LoadScene("EndGame", LoadSceneMode.Single);
                return true;
            }

            return false;
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
                NetworkManager.Singleton.OnClientDisconnectCallback -= Disconnect;
            }
        }
    }
}