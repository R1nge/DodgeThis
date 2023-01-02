using Unity.Netcode;

namespace Character.Fps
{
    public class CharacterStunFps : CharacterStun
    {
        private CharacterMovementFps _characterMovementFps;
        private CharacterCameraFps _characterCameraFps;

        private void Awake()
        {
            _characterMovementFps = GetComponent<CharacterMovementFps>();
            _characterCameraFps = GetComponent<CharacterCameraFps>();
        }

        [ServerRpc(RequireOwnership = false)]
        public override void StunServerRpc()
        {
            base.StunServerRpc();
            _characterMovementFps.StunServerRpc();
            _characterCameraFps.StunServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        protected override void ResetStunServerRpc()
        {
            base.ResetStunServerRpc();
            _characterMovementFps.ResetStunServerRpc();
            _characterCameraFps.ResetStunServerRpc();
        }
    }
}