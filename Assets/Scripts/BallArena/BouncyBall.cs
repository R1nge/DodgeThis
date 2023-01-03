using Character;
using Unity.Netcode;
using UnityEngine;

namespace BallArena
{
    public class BouncyBall : NetworkBehaviour
    {
        [SerializeField] private float forceMultiplier;
        private Rigidbody _rigidbody;
        private NetworkVariable<Vector3> _velocity;

        private void Awake()
        {
            _velocity = new NetworkVariable<Vector3>();
            _velocity.OnValueChanged += OnValueChanged;
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnValueChanged(Vector3 previousvalue, Vector3 newvalue)
        {
            _rigidbody.AddForce(newvalue);
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;
            var dir = new Vector3(Random.value, 0, Random.value) * forceMultiplier;
            _velocity.Value = dir;
            _rigidbody.AddForce(dir);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!IsServer) return;
            if (collision.transform.TryGetComponent(out CharacterState character))
            {
                if (!character.GetComponent<NetworkObject>().IsSpawned || character == null) return;
                character.Kill();
            }

            if (!collision.transform.CompareTag("Bouncable")) return;
            _velocity.Value = collision.contacts[0].normal * forceMultiplier;
        }
    }
}