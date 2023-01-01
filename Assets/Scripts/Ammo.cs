using Character;
using Unity.Netcode;
using UnityEngine;

public class Ammo : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out CharacterWeapon weapon))
        {
            weapon.AddAmmoServerRpc();
            Destroy(gameObject);
        }
    }
}