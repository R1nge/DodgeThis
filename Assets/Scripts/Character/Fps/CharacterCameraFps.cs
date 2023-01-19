using Unity.Netcode;
using UnityEngine;

namespace Character.Fps
{
    public class CharacterCameraFps : NetworkBehaviour
    {
        [SerializeField] private float lookSpeed = 2.0f;
        [SerializeField] private float lookXLimit = 90.0f;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private NetworkVariable<bool> canMove;
        private float _rotationX;

        [ServerRpc(RequireOwnership = false)]
        public void StunServerRpc() => canMove.Value = false;

        [ServerRpc(RequireOwnership = false)]
        public void ResetStunServerRpc() => canMove.Value = true;

        private void Awake()
        {
            canMove = new NetworkVariable<bool>(true);
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                playerCamera.enabled = false;
                playerCamera.GetComponent<AudioListener>().enabled = false;
            }
            else
            {
                playerCamera.enabled = true;
                playerCamera.GetComponent<AudioListener>().enabled = true;
            }
        }

        private void Update()
        {
            if (!IsOwner) return;
            if (!canMove.Value) return;
            if (IsOwnedByServer)
            {
                Rotate();
            }
            else
            {
                RotateServerRpc();
            }
        }


        private void Rotate()
        {
            if (canMove.Value)
            {
                _rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
                _rotationX = Mathf.Clamp(_rotationX, -lookXLimit, lookXLimit);
                playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            }
        }

        [ServerRpc]
        private void RotateServerRpc()
        {
            if (canMove.Value)
            {
                _rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
                _rotationX = Mathf.Clamp(_rotationX, -lookXLimit, lookXLimit);
                playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            }
        }
    }
}