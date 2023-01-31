using System.Collections.Generic;
using UnityEngine;

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
                throw new System.Exception("Multiple LobbySingleton defined!");
            }

            DontDestroyOnLoad(gameObject);
            Instance = this;
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
                Score = _lobbyPlayers[index].Score + amount
            };
        }
    }
}