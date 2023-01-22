using System.Collections;
using Shared;
using Unity.Netcode;
using UnityEngine;

namespace FallingEyeBalls
{
    public class EyeBallsTimer : NetworkBehaviour
    {
        [SerializeField] private int startTime;
        private NetworkVariable<int> _currentTime;
        private EyeBallsTimerUI _eyeBallsTimerUI;
        private GameState _gameState;
        private YieldInstruction _yield;

        private void Awake()
        {
            _eyeBallsTimerUI = GetComponent<EyeBallsTimerUI>();
            _gameState = FindObjectOfType<GameState>();
            _gameState.OnGameStarted += OnGameStarted;
            _yield = new WaitForSeconds(1f);
            _currentTime = new NetworkVariable<int>(startTime);
            _currentTime.OnValueChanged += OnValueChanged;
        }

        private void OnGameStarted()
        {
            if (!IsServer) return;
            StartCoroutine(Timer_c());
        }

        private void OnValueChanged(int _, int newValue) => _eyeBallsTimerUI.UpdateUIServerRpc(newValue);

        private IEnumerator Timer_c()
        {
            while (enabled)
            {
                if (_currentTime.Value <= 0)
                {
                    _gameState.EndGameServerRpc();
                    yield break;
                }

                yield return _yield;

                _currentTime.Value--;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _gameState.OnGameStarted -= OnGameStarted;
        }
    }
}