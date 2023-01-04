using Unity.Netcode;
using UnityEngine;

namespace Crane
{
    public class CranePlayer : NetworkBehaviour
    {
        [SerializeField] private float rayDistance;
        [SerializeField] private Camera playerCamera;
        private DropSpawner _dropSpawner;

        private void Awake() => _dropSpawner = FindObjectOfType<DropSpawner>();

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                playerCamera.enabled = true;
                playerCamera.GetComponent<AudioListener>().enabled = true;
            }
            else
            {
                playerCamera.enabled = false;
                playerCamera.GetComponent<AudioListener>().enabled = false;
            }
        }

        private void Update()
        {
            if (!IsOwner) return;
            Drag();
            Drop();
        }

        private void Drag()
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.SphereCast(ray, 2f, out var hit, rayDistance))
                {
                    if (hit.transform.TryGetComponent(out Draggable draggable))
                    {
                        if (draggable.CanDrag())
                        {
                            var pos = new Vector3(hit.point.x, draggable.transform.position.y, hit.point.z);

                            if (IsServer)
                            {
                                draggable.transform.position = pos;
                            }
                            else
                            {
                                if (draggable.TryGetComponent(out NetworkObject net))
                                {
                                    DragServerRpc(net, pos);
                                }
                            }
                        }
                    }
                }
            }
        }

        [ServerRpc]
        private void DragServerRpc(NetworkObjectReference obj, Vector3 pos)
        {
            if (obj.TryGet(out NetworkObject net))
            {
                net.transform.position = pos;
            }
        }

        private void Drop()
        {
            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.SphereCast(ray, 2f, out var hit, rayDistance))
                {
                    if (hit.transform.TryGetComponent(out Draggable draggable))
                    {
                        if (!draggable.CanDrag()) return;
                        draggable.DropServerRpc();
                        StartCoroutine(_dropSpawner.SpawnDrop_c());
                    }
                }
            }
        }
    }
}