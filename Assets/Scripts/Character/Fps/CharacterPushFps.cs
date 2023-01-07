using Unity.Netcode;
using UnityEngine;

namespace Character.Fps
{
    public class CharacterPushFps : NetworkBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float distance;
        private NetworkVariable<bool> _canPush;

        private void Awake() => _canPush = new NetworkVariable<bool>();

        private void Update()
        {
            if (!IsOwner) return;
            if (!Input.GetMouseButtonDown(0)) return;
            if (!_canPush.Value) return;
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out var hit, distance))
            {
                if (hit.transform.TryGetComponent(out Rigidbody player))
                {
                    PushServerRpc();
                }
            }
        }

        [ServerRpc]
        private void PushServerRpc() => _canPush.Value = false;

        [ServerRpc(RequireOwnership = false)]
        public void ResetAttackServerRpc() => _canPush.Value = true;
    }
}