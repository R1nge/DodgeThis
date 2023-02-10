using System;
using System.Collections;
using Shared;
using Unity.Netcode;
using UnityEngine;

namespace Misc
{
    public class CountDown : NetworkBehaviour
    {
        [SerializeField] private int timer;
        private NetworkVariable<int> _timer;
        private GameState _gameState;

        public event Action<int> OnTimeChangedEvent;

        private void Awake()
        {
            _gameState = FindObjectOfType<GameState>();
            _timer = new NetworkVariable<int>(timer);
            _timer.OnValueChanged += (_, newValue) => { OnTimeChangedEvent?.Invoke(newValue); };
        }

        private void Start()
        {
            OnTimeChangedEvent?.Invoke(_timer.Value);
            if (!IsServer) return;
            StartCoroutine(Count());
        }

        private IEnumerator Count()
        {
            yield return new WaitForSeconds(.5f);
            while (_timer.Value > 0)
            {
                yield return new WaitForSeconds(.5f);
                _timer.Value--;
            }

            _gameState.StartGameServerRpc();
        }
    }
}