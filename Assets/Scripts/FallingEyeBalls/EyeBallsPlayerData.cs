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

        private void OnScoreChanged(int _, int newValue) => _playerDataUI.UpdateUIServerRpc(newValue);

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