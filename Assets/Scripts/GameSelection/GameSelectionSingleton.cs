using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

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
        }

        [ServerRpc(RequireOwnership = false)]
        public void SelectGameServerRpc(int index)
        {
            if (!games[index].IsSelected)
            {
                games[index] = new Games
                {
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

        public GamesUI GetGamesUI(int index) => gamesUI[index];

        public List<Games> GetGames() => games;

        public List<Games> GetSelectedGames() => selectedGames;
    }
}