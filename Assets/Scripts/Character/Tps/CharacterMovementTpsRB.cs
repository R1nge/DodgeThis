using Unity.Netcode;
using UnityEngine;

namespace Character.Tps
{
    public class CharacterMovementTpsRB : NetworkBehaviour
    {
        [SerializeField] private NetworkVariable<float> movementSpeed;
        [SerializeField] private NetworkVariable<float> rotationSpeed;
        private NetworkVariable<bool> _canMove;
        private Vector3 _movementDirection;
        private Rigidbody _rigidbody;

        [ServerRpc(RequireOwnership = false)]
        public void StunServerRpc() => _canMove.Value = false;

        [ServerRpc(RequireOwnership = false)]
        public void ResetStunServerRpc() => _canMove.Value = true;

        private void Awake()
        {
            _canMove = new NetworkVariable<bool>(true);
            _rigidbody = GetComponent<Rigidbody>();
            NetworkManager.Singleton.NetworkTickSystem.Tick += OnTick;
        }

        private void OnTick()
        {
            if (!IsOwner) return;
            if (!_canMove.Value) return;
            if (IsOwnedByServer)
            {
                Rotate();
                Move();
            }
            else
            {
                var axis = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")) * movementSpeed.Value;
                _movementDirection = Vector3.forward * axis.x + Vector3.right * axis.y;
                RotateServerRpc(_movementDirection);
                MoveServerRpc(_movementDirection);
            }
        }

        private void Move()
        {
            var axis = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")) * movementSpeed.Value;
            _movementDirection = Vector3.forward * axis.x + Vector3.right * axis.y;
            _rigidbody.velocity =
                new Vector3(_movementDirection.x, _rigidbody.velocity.y, _movementDirection.z);
        }

        [ServerRpc]
        private void MoveServerRpc(Vector3 dir)
        {
            _rigidbody.velocity =
                new Vector3(dir.x, _rigidbody.velocity.y, dir.z);
        }

        private void Rotate()
        {
            if (_movementDirection.x != 0 || _movementDirection.z != 0)
            {
                var targetRot =
                    Quaternion.LookRotation(new Vector3(_movementDirection.x, 0, _movementDirection.z), Vector3.up);
                transform.rotation =
                    Quaternion.RotateTowards(transform.rotation, targetRot,
                        rotationSpeed.Value * NetworkManager.Singleton.NetworkTickSystem.TickRate);
            }
        }

        [ServerRpc]
        private void RotateServerRpc(Vector3 dir)
        {
            if (dir.x != 0 || dir.z != 0)
            {
                var targetRot =
                    Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z), Vector3.up);
                transform.rotation =
                    Quaternion.RotateTowards(transform.rotation, targetRot,
                        rotationSpeed.Value * NetworkManager.Singleton.NetworkTickSystem.TickRate);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (NetworkManager.Singleton && NetworkManager.Singleton.NetworkTickSystem != null)
            {
                NetworkManager.Singleton.NetworkTickSystem.Tick -= OnTick;
            }
        }
    }
}