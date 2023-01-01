using Unity.Netcode;
using UnityEngine;

namespace Crane
{
    public class CraneController : NetworkBehaviour
    {
        [SerializeField] private float movementSpeed;
        [SerializeField] private Vector2 limitX, limitZ;
        [SerializeField] private Lever leverX, leverZ;
        [SerializeField] private DropButton dropButton;
        [SerializeField] private ResetButton resetButton;
        [SerializeField] private Camera craneCamera;
        [SerializeField] private NetworkObject spawnPointPrefab;
        private NetworkObject _spawnPoint;

        private void Awake()
        {
            dropButton.OnButtonPressed += Drop;
            resetButton.OnButtonPressed += Reset;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                craneCamera.enabled = false;
                craneCamera.GetComponent<AudioListener>().enabled = false;
            }
            else
            {
                craneCamera.enabled = true;
                craneCamera.GetComponent<AudioListener>().enabled = true;
            }

            if (!IsServer) return;
            _spawnPoint = Instantiate(spawnPointPrefab, new Vector3(-2, -2.86f, 5.9f), Quaternion.identity);
            _spawnPoint.Spawn(true);
            _spawnPoint.transform.parent = transform.root;
            _spawnPoint.transform.localPosition = new Vector3(-2, -2.86f, 5.9f);
        }

        private void Reset()
        {
            leverX.transform.rotation = Quaternion.Euler(0, 0, 180);
            leverZ.transform.rotation = Quaternion.Euler(0, 0, 180);
        }

        private void Drop()
        {
            var drop = _spawnPoint.transform.GetChild(0);
            drop.parent = null;
            drop.GetComponent<Rigidbody>().isKinematic = false;
        }

        private void Update()
        {
            if (!IsServer) return;
            var position = _spawnPoint.transform.position;
            position += new Vector3(leverX.GetRot().x, 0, leverZ.GetRot().x) * movementSpeed;
            var pos = position;
            pos.x = Mathf.Clamp(position.x, limitX.x, limitX.y);
            pos.z = Mathf.Clamp(position.z, limitZ.x, limitZ.y);
            position = pos;
            _spawnPoint.transform.position = position;
        }

        private void OnDestroy()
        {
            dropButton.OnButtonPressed -= Drop;
            resetButton.OnButtonPressed -= Reset;
        }
    }
}