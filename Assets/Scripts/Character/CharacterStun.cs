using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public class CharacterStun : NetworkBehaviour
    {
        [SerializeField] private GameObject stunVfx;
        [SerializeField] private float yOffset;
        private CharacterMovement _characterMovement;
        private CharacterCamera _characterCamera;
        private GameObject _vfx;

        private void Awake()
        {
            _characterMovement = GetComponent<CharacterMovement>();
            _characterCamera = GetComponent<CharacterCamera>();
        }

        [ServerRpc(RequireOwnership = false)]
        public void StunServerRpc()
        {
            _characterMovement.StunServerRpc();
            _characterCamera.StunServerRpc();
            SpawnVfxServerRpc();
            Invoke(nameof(ResetStunServerRpc), 10f);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnVfxServerRpc()
        {
            var pos = transform.position;
            pos.y += yOffset;
            _vfx = Instantiate(stunVfx, pos, Quaternion.identity);
            _vfx.GetComponent<NetworkObject>().Spawn(true);
            _vfx.transform.parent = transform;
        }


        [ServerRpc(RequireOwnership = false)]
        private void ResetStunServerRpc()
        {
            _characterMovement.ResetStunServerRpc();
            _characterCamera.ResetStunServerRpc();
            Destroy(_vfx);
        }
    }
}