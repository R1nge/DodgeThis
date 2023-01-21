using System.Collections.Generic;
using Lobby;
using UnityEngine;

namespace Shared
{
    public class LobbySingleton : MonoBehaviour
    {
        public static LobbySingleton Instance { get; private set; }

        [SerializeField] private List<LobbyPlayerState> _lobbyPlayers;

        private void Awake()
        {
            _lobbyPlayers = new List<LobbyPlayerState>();
            if (Instance != null)
            {
                throw new System.Exception("Multiple LobbySingleton defined!");
            }

            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        public void ResetPlayerList() => _lobbyPlayers = new List<LobbyPlayerState>();

        public List<LobbyPlayerState> GetPlayersList() => _lobbyPlayers;

        public void AddScore(int index, int amount)
        {
            _lobbyPlayers[index] = new LobbyPlayerState
            {
                ClientId = _lobbyPlayers[index].ClientId,
                PlayerName = _lobbyPlayers[index].PlayerName,
                SkinIndex = _lobbyPlayers[index].SkinIndex,
                IsReady = _lobbyPlayers[index].IsReady,
                Score = _lobbyPlayers[index].Score + amount
            };
        }
    }
}