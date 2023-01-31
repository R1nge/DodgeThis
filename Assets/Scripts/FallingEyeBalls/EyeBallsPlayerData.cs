using Shared;
using Unity.Netcode;
using UnityEngine;

namespace FallingEyeBalls
{
    public class EyeBallsPlayerData : NetworkBehaviour
    {
        private NetworkVariable<int> _currentScore;
        private EyeBallsPlayerDataUI _playerDataUI;

        private void Awake()
        {
            _currentScore = new NetworkVariable<int>();
            _currentScore.OnValueChanged += OnScoreChanged;
            _playerDataUI = FindObjectOfType<EyeBallsPlayerDataUI>();
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
                _playerDataUI.UpdateNicknameServerRpc(players[i].Nickname);
                UpdateNicknamesClientRpc(players[i].Nickname);
            }
        }

        [ClientRpc]
        private void UpdateNicknamesClientRpc(NetworkString s)
        {
            _playerDataUI.UpdateNicknameServerRpc(s);
        }

        private void OnScoreChanged(int _, int newValue) => _playerDataUI.UpdateScoreServerRpc(newValue);

        public int CurrentScore => _currentScore.Value;

        private void Update()
        {
            if (!IsOwner) return;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                IncreaseScoreServerRpc();
            }
        }

        [ServerRpc]
        private void IncreaseScoreServerRpc() => _currentScore.Value++;
    }
}