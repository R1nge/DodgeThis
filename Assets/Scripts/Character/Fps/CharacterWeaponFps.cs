using Unity.Netcode;
using UnityEngine;

namespace Character.Fps
{
    public class CharacterWeaponFps : NetworkBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float distance;
        private NetworkVariable<bool> _canAttack;

        private void Awake() => _canAttack = new NetworkVariable<bool>();

        private void Update()
        {
            if (!IsOwner) return;
            if (!Input.GetMouseButtonDown(0)) return;
            if (!_canAttack.Value) return;
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out var hit, distance))
            {
                if (hit.transform.TryGetComponent(out CharacterStun stun))
                {
                    stun.StunServerRpc();
                    AttackServerRpc();
                }
            }
        }

        [ServerRpc]
        private void AttackServerRpc() => _canAttack.Value = false;

        [ServerRpc(RequireOwnership = false)]
        public void ResetAttackServerRpc() => _canAttack.Value = true;
    }
}