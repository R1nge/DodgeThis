using Unity.Netcode;
using UnityEngine;

namespace Character
{
    public abstract class CharacterStun : NetworkBehaviour
    {
        [SerializeField] private GameObject stunVfx;
        [SerializeField] private GameObject stunSound;
        [SerializeField] private float stunDuration;
        [SerializeField] private float yOffset;
        private GameObject _vfx;

        [ServerRpc(RequireOwnership = false)]
        public virtual void StunServerRpc()
        {
            SpawnVfxServerRpc();
            SpawnSoundServerRpc();
            Invoke(nameof(ResetStunServerRpc), stunDuration);
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
        private void SpawnSoundServerRpc()
        {
            var sound = Instantiate(stunSound, transform.position, Quaternion.identity);
            sound.GetComponent<NetworkObject>().Spawn(true);
        }


        [ServerRpc(RequireOwnership = false)]
        protected virtual void ResetStunServerRpc() => Destroy(_vfx);
    }
}