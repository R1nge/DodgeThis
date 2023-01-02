using Unity.Netcode;

namespace Character.Fps
{
    public class CharacterStunFps : CharacterStun
    {
        private CharacterMovementFpsCC _characterMovementFpsCc;
        private CharacterCameraFps _characterCameraFps;

        private void Awake()
        {
            _characterMovementFpsCc = GetComponent<CharacterMovementFpsCC>();
            _characterCameraFps = GetComponent<CharacterCameraFps>();
        }

        [ServerRpc(RequireOwnership = false)]
        public override void StunServerRpc()
        {
            base.StunServerRpc();
            _characterMovementFpsCc.StunServerRpc();
            _characterCameraFps.StunServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        protected override void ResetStunServerRpc()
        {
            base.ResetStunServerRpc();
            _characterMovementFpsCc.ResetStunServerRpc();
            _characterCameraFps.ResetStunServerRpc();
        }
    }
}