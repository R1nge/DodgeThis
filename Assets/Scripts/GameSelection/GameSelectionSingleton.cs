using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameSelection
{
    public class GameSelectionSingleton : MonoBehaviour
    {
        public static GameSelectionSingleton Instance { get; private set; }
        [SerializeField] private GamesUI[] gamesUI;
        [SerializeField] private List<Games> games, selectedGames;

        private void Awake()
        {
            for (int i = 0; i < gamesUI.Length; i++)
            {
                games.Add(new Games(
                    gamesUI[i].title,
                    gamesUI[i].sceneName,
                    false,
                    false
                ));
            }

            if (Instance != null)
            {
                throw new Exception("Multiple GameSelectionSingleton defined!");
            }

            DontDestroyOnLoad(gameObject);
            Instance = this;

            SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
        }

        private void SceneManagerOnsceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == "MainMenu")
            {
                ResetSelectedGames();
            }
        }

        public void SelectGame(int index)
        {
            if (!games[index].IsSelected)
            {
                games[index] = new Games
                {
                    Title = gamesUI[index].title,
                    IsSelected = true,
                    SceneName = games[index].SceneName,
                    HasBeenPlayed = false
                };

                selectedGames.Add(games[index]);
            }
        }

        public void ResetSelectedGames()
        {
            for (int i = 0; i < games.Count; i++)
            {
                games[i] = new Games
                {
                    IsSelected = false,
                    SceneName = games[i].SceneName,
                    HasBeenPlayed = false
                };
            }

            selectedGames = new List<Games>();
        }

        public GamesUI[] GetGamesUI() => gamesUI;

        public List<Games> GetGames() => games;

        public List<Games> GetSelectedGames() => selectedGames;
    }
}