using System;
using UnityEngine;

namespace Character
{
    public class CharacterCamera : MonoBehaviour
    {
        [SerializeField] private float lookSpeed = 2.0f;
        [SerializeField] private float lookXLimit = 90.0f;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private bool canMove = true;
        private float _rotationX;


        private void Update()
        {
            if (canMove)
            {
                _rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
                _rotationX = Mathf.Clamp(_rotationX, -lookXLimit, lookXLimit);
                playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            }
        }
    }
}