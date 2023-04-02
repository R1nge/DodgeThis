using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameSelection
{
    public class GameSelectionSingleton : MonoBehaviour
    {
        public static GameSelectionSingleton Instance { get; private set; }
        [SerializeField] private GamesUI[] gamesUI;
        private readonly List<Games> _games = new();
        private List<Games> _selectedGames = new();

        private void Awake()
        {
            for (int i = 0; i < gamesUI.Length; i++)
            {
                _games.Add(new Games(
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
            if (!_games[index].IsSelected)
            {
                _games[index] = new Games
                {
                    Title = gamesUI[index].title,
                    IsSelected = true,
                    SceneName = _games[index].SceneName,
                    HasBeenPlayed = false
                };

                _selectedGames.Add(_games[index]);
            }
        }

        public void ResetSelectedGames()
        {
            for (int i = 0; i < _games.Count; i++)
            {
                _games[i] = new Games
                {
                    IsSelected = false,
                    SceneName = _games[i].SceneName,
                    HasBeenPlayed = false
                };
            }

            _selectedGames = new List<Games>();
        }

        public GamesUI[] GetGamesUI() => gamesUI;

        public List<Games> GetGames() => _games;

        public List<Games> GetSelectedGames() => _selectedGames;
    }
}