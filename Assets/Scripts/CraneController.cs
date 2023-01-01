using System;
using UnityEngine;

public class CraneController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private Transform drop;
    [SerializeField] private Lever leverX, leverZ;
    [SerializeField] private DropButton dropButton;

    private void Awake() => dropButton.OnButtonPressed += Drop;

    private void Drop()
    {
        drop.GetComponent<Rigidbody>().isKinematic = false;
        drop = null;
    }

    private void Update()
    {
        if(drop == null) return;
        drop.transform.position += new Vector3(leverX.GetRot(), 0, leverZ.GetRot()) * movementSpeed;
    }

    private void OnDestroy() => dropButton.OnButtonPressed -= Drop;
}