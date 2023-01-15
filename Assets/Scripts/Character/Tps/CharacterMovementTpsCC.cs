using Unity.Netcode;
using UnityEngine;

namespace Character.Tps
{
    public class CharacterMovementTpsCC : NetworkBehaviour
    {
        [SerializeField] private NetworkVariable<float> movementSpeed;
        [SerializeField] private NetworkVariable<float> rotationSpeed;
        [SerializeField] private float jumpSpeed = 8.0f;
        [SerializeField] private float gravity = 20.0f;
        [SerializeField] private bool canJump;
        private NetworkVariable<bool> _canMove;
        private Vector3 _moveDirection = Vector3.zero;
        private float _curSpeedX, _curSpeedY;
        private CharacterController _characterController;

        [ServerRpc(RequireOwnership = false)]
        public void StunServerRpc() => _canMove.Value = false;

        [ServerRpc(RequireOwnership = false)]
        public void ResetStunServerRpc() => _canMove.Value = true;

        private void Awake()
        {
            _canMove = new NetworkVariable<bool>(true);
            _characterController = GetComponent<CharacterController>();
            NetworkManager.Singleton.NetworkTickSystem.Tick += OnTick;
        }

        private void OnTick()
        {
            if (!IsOwner) return;
            if (IsOwnedByServer)
            {
                GetInput();
                Move();
                Rotate();
            }
            else
            {
                GetInput();

                _moveDirection = Vector3.forward * _curSpeedX + Vector3.right * _curSpeedY;
                float movementDirectionY = _moveDirection.y;

                if (Input.GetButton("Jump") && _characterController.isGrounded && _canMove.Value && canJump)
                {
                    _moveDirection.y = jumpSpeed;
                }
                else
                {
                    _moveDirection.y = movementDirectionY;
                }

                if (!_characterController.isGrounded)
                {
                    _moveDirection.y -= gravity;
                }

                MoveServerRpc(_moveDirection);
                RotateServerRpc(_moveDirection);
            }
        }


        private void Move()
        {
            float movementDirectionY = _moveDirection.y;
            _moveDirection = Vector3.forward * _curSpeedX + Vector3.right * _curSpeedY;

            if (Input.GetButton("Jump") && _characterController.isGrounded && _canMove.Value && canJump)
            {
                _moveDirection.y = jumpSpeed;
            }
            else
            {
                _moveDirection.y = movementDirectionY;
            }

            if (!_characterController.isGrounded)
            {
                _moveDirection.y -= gravity;
            }

            _characterController.Move(_moveDirection / NetworkManager.Singleton.NetworkTickSystem.TickRate);
        }

        [ServerRpc]
        private void MoveServerRpc(Vector3 dir)
        {
            _characterController.Move(dir / NetworkManager.Singleton.NetworkTickSystem.TickRate);
        }

        private void GetInput()
        {
            if (_canMove.Value)
            {
                _curSpeedX = Input.GetAxis("Vertical") * movementSpeed.Value;
                _curSpeedY = Input.GetAxis("Horizontal") * movementSpeed.Value;
            }
        }

        private void Rotate()
        {
            if (_moveDirection.x != 0 || _moveDirection.z != 0)
            {
                var targetRot =
                    Quaternion.LookRotation(new Vector3(_moveDirection.x, 0, _moveDirection.z), Vector3.up);
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
            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.NetworkTickSystem.Tick -= OnTick;
            }
        }
    }
}