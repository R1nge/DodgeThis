using System;
using UnityEngine;

public class Lever : MonoBehaviour
{
    [SerializeField] private float rotSpeed;
    [SerializeField] private float min, max;

    public float GetRot()
    {
        var rot = transform.rotation.eulerAngles.z;
        print("Before " + rot);
        if (transform.rotation.eulerAngles.z < 180)
        {
            rot = -transform.rotation.eulerAngles.z;
        }
        else if (Math.Abs(transform.rotation.eulerAngles.z - 180) < 20f)
        {
            rot = 0;
        }

        if (Math.Abs(rot - 1) < .2f || Math.Abs(rot - (-1)) < 20f)
        {
            rot = 0;
        }

        print(rot);
        return rot;
    }

    private void OnMouseDrag()
    {
        float xRot = Input.GetAxis("Mouse Y") * rotSpeed;
        transform.Rotate(Vector3.forward, xRot * rotSpeed);
        var rot = transform.rotation.eulerAngles;
        rot.z = Mathf.Clamp(rot.z, min, max);
        print(rot.z);
        transform.rotation = Quaternion.Euler(rot);
    }
}