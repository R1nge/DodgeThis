using System;
using System.Linq;
using Shared;
using Unity.Netcode;
using UnityEngine;

namespace EndGame
{
    public class EndGamePlayerSpawner : PlayerSpawner
    {
        [SerializeField] private Transform[] positions;
        private int _lastPosition;
        
        private void Start() => OnGameStarted();

        protected override void SpawnPlayer(ulong ID)
        {
            if (!IsServer) return;
            var players = LobbySingleton.Instance.GetPlayersList();
            var sorted = players.OrderByDescending(x => x.Score).ToArray();

            for (int i = 0; i < sorted.Length; i++)
            {
                if (ID == sorted[i].ClientId)
                {
                    _lastPosition = i % 4;
                    var controller = skins.GetController(4);
                    var controllerPos = positions[_lastPosition].position;
                    var controllerRot = Quaternion.Euler(0, 180, 0);
                    var controllerInst = Instantiate(controller, controllerPos, controllerRot);
                    controllerInst.GetComponent<NetworkObject>().SpawnWithOwnership(ID, true);
                    controllerInst.transform.position = positions[_lastPosition].position;
                    
                    var skin = skins.GetSkin(sorted[i].SkinIndex);
                    var skinPos = controllerInst.transform.position + skins.GetOffset(i);
                    var skinRot = Quaternion.Euler(0, 180, 0);
                    var skinInst = Instantiate(skin, skinPos, skinRot);
                    skinInst.GetComponent<NetworkObject>().SpawnWithOwnership(ID, true);
                    skinInst.transform.parent = controllerInst.transform;
                    skinInst.transform.localPosition = skins.GetOffset(i);
                }
            }
        }
    }
}