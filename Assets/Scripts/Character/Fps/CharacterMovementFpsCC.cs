using Unity.Netcode;
using UnityEngine;

namespace Character.Fps
{
    public class CharacterMovementFpsCC : NetworkBehaviour
    {
        [SerializeField] private NetworkVariable<float> walkingSpeed = new(7.5f);
        [SerializeField] private NetworkVariable<float> runningSpeed = new(11.5f);
        [SerializeField] private NetworkVariable<float> jumpSpeed = new(8.0f);
        [SerializeField] private NetworkVariable<float> gravity = new(20.0f);
        [SerializeField] private NetworkVariable<bool> canJump = new(true);
        private NetworkVariable<bool> _canMove;
        private Vector3 _moveDirection = Vector3.zero;
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
            if (!_canMove.Value) return;
            if (IsOwnedByServer)
            {
                GetInput();
                Move();
            }
            else
            {
                GetInput();
                MoveServerRpc(_moveDirection);
            }
        }

        private void Move()
        {
            _characterController.Move(_moveDirection / NetworkManager.Singleton.NetworkTickSystem.TickRate);
        }

        private void GetInput()
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float curSpeedX =
                _canMove.Value ? (isRunning ? runningSpeed.Value : walkingSpeed.Value) * Input.GetAxis("Vertical") : 0;
            float curSpeedY =
                _canMove.Value
                    ? (isRunning ? runningSpeed.Value : walkingSpeed.Value) * Input.GetAxis("Horizontal")
                    : 0;
            float movementDirectionY = _moveDirection.y;
            _moveDirection = forward * curSpeedX + right * curSpeedY;

            if (Input.GetButton("Jump") && _canMove.Value && canJump.Value && _characterController.isGrounded)
            {
                _moveDirection.y = jumpSpeed.Value;
            }
            else
            {
                _moveDirection.y = movementDirectionY;
            }

            if (!_characterController.isGrounded)
            {
                _moveDirection.y -= gravity.Value;
            }
        }

        [ServerRpc]
        private void MoveServerRpc(Vector3 dir)
        {
            _characterController.Move(dir / NetworkManager.Singleton.NetworkTickSystem.TickRate);
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