using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public class CharacterWeapon : NetworkBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float distance;
        private NetworkVariable<bool> _canShoot;

        private void Awake() => _canShoot = new NetworkVariable<bool>();

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            if (!_canShoot.Value) return;
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out var hit, distance))
            {
                if (hit.transform.TryGetComponent(out CharacterStun stun))
                {
                    stun.StunServerRpc();
                    ShootServerRpc();
                }
            }
        }

        [ServerRpc]
        private void ShootServerRpc() => _canShoot.Value = false;

        [ServerRpc(RequireOwnership = false)]
        public void AddAmmoServerRpc() => _canShoot.Value = true;
    }
}