using UnityEngine;

namespace Character
{
    public class CharacterWeapon : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float distance;

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out var hit, distance))
            {
                if (hit.transform.TryGetComponent(out CharacterStun stun))
                {
                    stun.StunServerRpc();
                }
            }
        }
    }
}