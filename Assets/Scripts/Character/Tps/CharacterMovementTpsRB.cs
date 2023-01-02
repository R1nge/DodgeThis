using UnityEngine;

namespace Character.Tps
{
    public class CharacterMovementTpsRB : MonoBehaviour
    {
        [SerializeField] private float movementSpeed;
        [SerializeField] private float rotationSpeed;
        private Vector3 _movementDirection;
        private Rigidbody _rigidbody;

        private void Awake() => _rigidbody = GetComponent<Rigidbody>();

        private void FixedUpdate()
        {
            Rotate();
            Move();
        }

        private void Move()
        {
            var axis = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")) * movementSpeed;
            _movementDirection = Vector3.forward * axis.x + Vector3.right * axis.y;
            _rigidbody.velocity =
                new Vector3(_movementDirection.x, _rigidbody.velocity.y, _movementDirection.z);
        }

        private void Rotate()
        {
            if (_movementDirection.x != 0 || _movementDirection.z != 0)
            {
                var targetRot = 
                    Quaternion.LookRotation(new Vector3(_movementDirection.x, 0, _movementDirection.z), Vector3.up);
                transform.rotation =
                    Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }
    }
}