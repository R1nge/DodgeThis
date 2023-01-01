using UnityEngine;

namespace Character
{
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] private float walkingSpeed = 7.5f;
        [SerializeField] private float runningSpeed = 11.5f;
        [SerializeField] private float jumpSpeed = 8.0f;
        [SerializeField] private float gravity = 20.0f;
        [SerializeField] private bool canMove = true;
        private Vector3 _moveDirection = Vector3.zero;
        private CharacterController _characterController;
        
        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
            float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
            float movementDirectionY = _moveDirection.y;
            _moveDirection = forward * curSpeedX + right * curSpeedY;

            if (Input.GetButton("Jump") && canMove && _characterController.isGrounded)
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