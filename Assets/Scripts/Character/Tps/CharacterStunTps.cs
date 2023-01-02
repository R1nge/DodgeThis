using Unity.Netcode;

namespace Character.Tps
{
    public class CharacterStunTps : CharacterStun
    {
        private CharacterMovementTpsCC _characterMovementTpsCc;

        private void Awake()
        {
            _characterMovementTpsCc = GetComponent<CharacterMovementTpsCC>();
        }

        [ServerRpc(RequireOwnership = false)]
        public override void StunServerRpc()
        {
            base.StunServerRpc();
            _characterMovementTpsCc.StunServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        protected override void ResetStunServerRpc()
        {
            base.ResetStunServerRpc();
            _characterMovementTpsCc.ResetStunServerRpc();
        }
    }
}