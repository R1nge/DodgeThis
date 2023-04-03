using Shared;
using Unity.Netcode;
using UnityEngine;

namespace BallArena
{
    public class PlayerSpawnerBallArena : PlayerSpawner
    {
        [SerializeField] private Transform[] positions;
        private int _lastPosition;

        protected override void SpawnPlayer(ulong ID)
        {
            if (!IsServer) return;
            for (int i = 0; i < LobbySingleton.Instance.GetPlayersList().Count; i++)
            {
                if (ID == LobbySingleton.Instance.GetPlayersList()[i].ClientId)
                {
                    var controllerInst = Instantiate(controller, positions[_lastPosition].position,
                        Quaternion.identity);
                    controllerInst.GetComponent<NetworkObject>().SpawnWithOwnership(ID, true);
                    controllerInst.transform.position = positions[_lastPosition].position;
                    var skin = Instantiate(skins.GetSkin(LobbySingleton.Instance.GetPlayersList()[i].SkinIndex),
                        controllerInst.transform.position + skins.GetOffset(i), Quaternion.identity);
                    skin.GetComponent<NetworkObject>().SpawnWithOwnership(ID, true);
                    skin.transform.parent = controllerInst.transform;
                    skin.transform.localPosition = skins.GetOffset(i);
                }
            }

            _lastPosition++;
        }
    }
}