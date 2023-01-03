using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace NumbersFloor
{
    public class Tile : NetworkBehaviour
    {
        [SerializeField] private Materials materials;
        private NetworkVariable<int> _currentNumber;
        private BoxCollider _boxCollider;
        private MeshRenderer _meshRenderer;
        private GameState _gameState;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _boxCollider = GetComponent<BoxCollider>();
            _currentNumber = new NetworkVariable<int>();
            _gameState = FindObjectOfType<GameState>();
            _gameState.OnGameStarted += OnGameStarted;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            _currentNumber.OnValueChanged += (_, newValue) => { UpdateClientRpc(newValue); };
            _currentNumber.Value = Random.Range(3, 9);
        }

        private void OnGameStarted()
        {
            if (!IsServer) return;
            StartCoroutine(DecreaseNumber());
        }

        private IEnumerator DecreaseNumber()
        {
            while (enabled)
            {
                yield return new WaitForSeconds(1);
                _currentNumber.Value--;

                if (_currentNumber.Value <= 0)
                {
                    _boxCollider.enabled = false;
                    DisableColliderClientRpc();
                    yield return new WaitForSeconds(1);
                    _boxCollider.enabled = true;
                    EnableColliderClientRpc();
                    _currentNumber.Value = Random.Range(3, 9);
                }
            }
        }

        [ClientRpc]
        private void DisableColliderClientRpc() => _boxCollider.enabled = false;

        [ClientRpc]
        private void EnableColliderClientRpc() => _boxCollider.enabled = true;

        [ClientRpc]
        private void UpdateClientRpc(int val)
        {
            if (val == 9)
            {
                _meshRenderer.material = materials.GetMaterial(9);
                _meshRenderer.material.color = materials.Purple;
            }
            else if (val == 8)
            {
                _meshRenderer.material = materials.GetMaterial(8);
                _meshRenderer.material.color = materials.DarkBlue;
            }
            else if (val == 7)
            {
                _meshRenderer.material = materials.GetMaterial(7);
                _meshRenderer.material.color = materials.Blue;
            }
            else if (val == 6)
            {
                _meshRenderer.material = materials.GetMaterial(6);
                _meshRenderer.material.color = materials.LightBlue;
            }
            else if (val == 5)
            {
                _meshRenderer.material = materials.GetMaterial(5);
                _meshRenderer.material.color = materials.Emerald;
            }
            else if (val == 4)
            {
                _meshRenderer.material = materials.GetMaterial(4);
                _meshRenderer.material.color = materials.LightGreen;
            }
            else if (val == 3)
            {
                _meshRenderer.material = materials.GetMaterial(3);
                _meshRenderer.material.color = materials.YellowGreen;
            }
            else if (val == 2)
            {
                _meshRenderer.material = materials.GetMaterial(2);
                _meshRenderer.material.color = materials.Orange;
            }
            else if (val == 1)
            {
                _meshRenderer.material = materials.GetMaterial(1);
                _meshRenderer.material.color = materials.DarkOrange;
            }
            else if (val == 0)
            {
                _meshRenderer.material = materials.GetMaterial(0);
                _meshRenderer.material.color = materials.Red;
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _gameState.OnGameStarted -= OnGameStarted;
        }
    }
}