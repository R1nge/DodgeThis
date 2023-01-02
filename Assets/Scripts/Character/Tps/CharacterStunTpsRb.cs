using Unity.Netcode;

namespace Character.Tps
{
    public class CharacterStunTpsRb : CharacterStun
    {
        private CharacterMovementTpsRB _characterMovementTpsRb;

        private void Awake()
        {
            _characterMovementTpsRb = GetComponent<CharacterMovementTpsRB>();
        }

        [ServerRpc(RequireOwnership = false)]
        public override void StunServerRpc()
        {
            base.StunServerRpc();
            _characterMovementTpsRb.StunServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        protected override void ResetStunServerRpc()
        {
            base.ResetStunServerRpc();
            _characterMovementTpsRb.ResetStunServerRpc();
        }
    }
}