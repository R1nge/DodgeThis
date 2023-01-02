using Character;
using Character.Fps;
using Unity.Netcode;
using UnityEngine;

public class Ammo : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterWeaponFps weapon))
        {
            weapon.ResetAttackServerRpc();
            Destroy(gameObject);
        }
    }
}