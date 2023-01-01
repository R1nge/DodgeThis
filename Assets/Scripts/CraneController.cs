using UnityEngine;

public class CraneController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Lever leverX, leverZ;
    [SerializeField] private DropButton dropButton;
    [SerializeField] private ResetButton resetButton;

    private void Awake()
    {
        dropButton.OnButtonPressed += Drop;
        resetButton.OnButtonPressed += Reset;
    }

    private void Reset()
    {
        leverX.transform.rotation = Quaternion.Euler(0, 0, 180);
        leverZ.transform.rotation = Quaternion.Euler(0, 0, 180);
    }

    private void Drop()
    {
        var drop = spawnPoint.GetChild(0);
        drop.parent = null;
        drop.GetComponent<Rigidbody>().isKinematic = false;
    }

    private void Update()
    {
        spawnPoint.transform.position += new Vector3(leverX.GetRot(), 0, leverZ.GetRot()) * movementSpeed;
    }

    private void OnDestroy()
    {
        dropButton.OnButtonPressed -= Drop;
        resetButton.OnButtonPressed -= Reset;
    }
}