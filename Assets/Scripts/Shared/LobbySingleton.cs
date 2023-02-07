using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Shared
{
    public class LobbySingleton : MonoBehaviour
    {
        public static LobbySingleton Instance { get; private set; }

        [SerializeField] private List<PlayerState> _lobbyPlayers;

        private void Awake()
        {
            _lobbyPlayers = new List<PlayerState>();
            if (Instance != null)
            {
                throw new Exception("Multiple LobbySingleton defined!");
            }

            DontDestroyOnLoad(gameObject);
            Instance = this;
            
            SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
        }

        private void SceneManagerOnsceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == "MainMenu")
            {
                ResetPlayerList();
            }
        }

        public void ResetPlayerList() => _lobbyPlayers = new List<PlayerState>();

        public List<PlayerState> GetPlayersList() => _lobbyPlayers;

        public void AddScore(int index, int amount)
        {
            _lobbyPlayers[index] = new PlayerState
            {
                ClientId = _lobbyPlayers[index].ClientId,
                Nickname = _lobbyPlayers[index].Nickname,
                SkinIndex = _lobbyPlayers[index].SkinIndex,
                IsReady = _lobbyPlayers[index].IsReady,
                Score = _lobbyPlayers[index].Score + amount,
                IsAlive = _lobbyPlayers[index].IsAlive,
            };
        }

        public void ReviveAllPlayers()
        {
            for (int i = 0; i < _lobbyPlayers.Count; i++)
            {
                _lobbyPlayers[i] = new PlayerState
                {
                    ClientId = _lobbyPlayers[i].ClientId,
                    Nickname = _lobbyPlayers[i].Nickname,
                    SkinIndex = _lobbyPlayers[i].SkinIndex,
                    IsReady = _lobbyPlayers[i].IsReady,
                    Score = _lobbyPlayers[i].Score,
                    IsAlive = true,
                };
            }
        }
    }
}