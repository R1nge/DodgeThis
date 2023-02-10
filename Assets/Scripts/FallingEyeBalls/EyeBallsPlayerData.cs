using System;
using Shared;
using Unity.Netcode;
using UnityEngine;

namespace FallingEyeBalls
{
    public class EyeBallsPlayerData : NetworkBehaviour
    {
        private int _localScore;
        private NetworkVariable<int> _currentScore;
        private GameState _gameState;

        public event Action<NetworkString> OnNicknameChangedEvent;
        public event Action<int> OnScoreChangedEvent;

        public event Action<int> OnLocalScoreChangedEvent;

        private void Awake()
        {
            _currentScore = new NetworkVariable<int>();
            _currentScore.OnValueChanged += OnScoreChanged;
            _gameState = FindObjectOfType<GameState>();
            _gameState.OnGameEnd += SetScore;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            UpdateNicknameServerRpc();
        }

        [ServerRpc]
        private void UpdateNicknameServerRpc(ServerRpcParams rpcParams = default)
        {
            var players = LobbySingleton.Instance.GetPlayersList();
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].ClientId != rpcParams.Receive.SenderClientId) continue;
                OnNicknameChangedEvent?.Invoke(players[i].Nickname);
                UpdateNicknamesClientRpc(players[i].Nickname);
            }
        }

        [ClientRpc]
        private void UpdateNicknamesClientRpc(NetworkString s)
        {
            OnNicknameChangedEvent?.Invoke(s);
        }

        private void OnScoreChanged(int _, int newValue) => OnScoreChangedEvent?.Invoke(newValue);

        public int CurrentScore => _currentScore.Value;

        private void Update()
        {
            if (!IsOwner) return;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                IncreaseScoreLocally();
            }
        }

        private void IncreaseScoreLocally()
        {
            _localScore++;
            OnLocalScoreChangedEvent?.Invoke(_localScore);
        }

        private void SetScore()
        {
            if (!IsOwner) return;
            SetScoreServerRpc(_localScore);
        }

        [ServerRpc]
        private void SetScoreServerRpc(int value)
        {
            _currentScore.Value = value;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _gameState.OnGameEnd -= SetScore;
        }
    }
}