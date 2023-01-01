using Unity.Netcode;

namespace Character
{
    public class CharacterStun : NetworkBehaviour
    {
        private CharacterMovement _characterMovement;
        private CharacterCamera _characterCamera;

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
            Invoke(nameof(ResetStunServerRpc), 10f);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ResetStunServerRpc()
        {
            _characterMovement.ResetStunServerRpc();
            _characterCamera.ResetStunServerRpc();
        }
    }
}