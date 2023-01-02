using Unity.Netcode;
using UnityEngine;

namespace Dodge
{
    public class Movement : NetworkBehaviour
    {
        [SerializeField] private Vector3 direction;
        [SerializeField] private float speed;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (!IsServer) return;
            _rigidbody.MovePosition(_rigidbody.position + direction * speed);
        }
    }
}