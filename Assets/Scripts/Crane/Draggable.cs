using Unity.Netcode;
using UnityEngine;

namespace Crane
{
    public class Draggable : NetworkBehaviour
    {
        private NetworkVariable<bool> _dropped;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _dropped = new NetworkVariable<bool>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        public bool CanDrag() => !_dropped.Value;

        [ServerRpc(RequireOwnership = false)]
        public void DropServerRpc()
        {
            _rigidbody.isKinematic = false;
            _dropped.Value = true;
        }
    }
}