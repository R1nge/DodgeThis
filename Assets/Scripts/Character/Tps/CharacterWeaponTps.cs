using Unity.Netcode;
using UnityEngine;

namespace Character.Tps
{
    public class CharacterWeaponTps : NetworkBehaviour
    {
        [SerializeField] private float distance;
        private NetworkVariable<bool> _canAttack;

        private void Awake() => _canAttack = new NetworkVariable<bool>(true);

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (!_canAttack.Value) return;
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out var hit, distance))
            {
                if (hit.transform.TryGetComponent(out CharacterStun stun))
                {
                    stun.StunServerRpc();
                    AttackServerRpc();
                    Invoke(nameof(ResetAttackServerRpc), 2f);
                }
            }
        }

        [ServerRpc]
        private void AttackServerRpc() => _canAttack.Value = false;

        [ServerRpc(RequireOwnership = false)]
        public void ResetAttackServerRpc() => _canAttack.Value = true;
    }
}