using Unity.Netcode;
using UnityEngine;

namespace Character.Tps
{
    public class CharacterMovementTpsCC : NetworkBehaviour
    {
        [SerializeField] private NetworkVariable<float> speed;
        [SerializeField] private float rotationSpeed;
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
        }

        private void Update()
        {
            if (!IsOwner) return;
            if (_canMove.Value)
            {
                _curSpeedX = Input.GetAxis("Vertical") * speed.Value;
                _curSpeedY = Input.GetAxis("Horizontal") * speed.Value;
            }

            float movementDirectionY = _moveDirection.y;
            _moveDirection = Vector3.forward * _curSpeedX + Vector3.right * _curSpeedY;

            Rotate();

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
                _moveDirection.y -= gravity * Time.deltaTime;
            }

            _characterController.Move(_moveDirection * Time.deltaTime);
        }

        private void Rotate()
        {
            if (_moveDirection != Vector3.zero)
            {
                var targetRot = Quaternion.LookRotation(_moveDirection, Vector3.up);
                transform.rotation =
                    Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }
    }
}