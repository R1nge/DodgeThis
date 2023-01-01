using UnityEngine;

public class CraneController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private Vector2 limitX, limitZ;
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
        var position = spawnPoint.transform.position;
        position += new Vector3(leverX.GetRot().x, 0, leverZ.GetRot().x) * movementSpeed;
        var pos = position;
        pos.x = Mathf.Clamp(position.x, limitX.x, limitX.y);
        pos.z = Mathf.Clamp(position.z, limitZ.x, limitZ.y);
        position = pos;
        spawnPoint.transform.position = position;
    }

    private void OnDestroy()
    {
        dropButton.OnButtonPressed -= Drop;
        resetButton.OnButtonPressed -= Reset;
    }
}