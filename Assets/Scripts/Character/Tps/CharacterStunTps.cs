using Unity.Netcode;

namespace Character.Tps
{
    public class CharacterStunTps : CharacterStun
    {
        private CharacterMovementTps _characterMovementTps;

        private void Awake()
        {
            _characterMovementTps = GetComponent<CharacterMovementTps>();
        }

        [ServerRpc(RequireOwnership = false)]
        public override void StunServerRpc()
        {
            base.StunServerRpc();
            _characterMovementTps.StunServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        protected override void ResetStunServerRpc()
        {
            base.ResetStunServerRpc();
            _characterMovementTps.ResetStunServerRpc();
        }
    }
}