using System;
using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public class CharacterMovement : NetworkBehaviour
    {
        [SerializeField] private float walkingSpeed = 7.5f;
        [SerializeField] private float runningSpeed = 11.5f;
        [SerializeField] private float jumpSpeed = 8.0f;
        [SerializeField] private float gravity = 20.0f;
        [SerializeField] private NetworkVariable<bool> canMove;
        private Vector3 _moveDirection = Vector3.zero;
        private CharacterController _characterController;

        [ServerRpc(RequireOwnership = false)]
        public void StunServerRpc() => canMove.Value = false;

        [ServerRpc(RequireOwnership = false)]
        public void ResetStunServerRpc() => canMove.Value = true;

        private void Awake()
        {
            canMove = new NetworkVariable<bool>(true);
        }

        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (!IsOwner) return;
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float curSpeedX = canMove.Value ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
            float curSpeedY =
                canMove.Value ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
            float movementDirectionY = _moveDirection.y;
            _moveDirection = forward * curSpeedX + right * curSpeedY;

            if (Input.GetButton("Jump") && canMove.Value && _characterController.isGrounded)
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
    }
}