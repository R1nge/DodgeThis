using Unity.Netcode;
using UnityEngine;

namespace Dodge
{
    public class MobsSpawner : NetworkBehaviour
    {
        [SerializeField] private GameObject mob, bigMob;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }
    }
}