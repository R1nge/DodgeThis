using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public class CharacterState : MonoBehaviour
    {
        private GameState _gameState;

        private void Awake()
        {
            _gameState = FindObjectOfType<GameState>();
        }

        public void Kill()
        {
            _gameState.TakePlaceServerRpc();
            if (TryGetComponent(out NetworkObject networkObject))
            {
                if (!networkObject.IsSpawned) return;
                networkObject.Despawn();
            }
        }
    }
}