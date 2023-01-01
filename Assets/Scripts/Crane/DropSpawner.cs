using System.Collections;
using UnityEngine;

namespace Crane
{
    public class DropSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject[] drops;
        [SerializeField] private Transform spawnPoint;
        private DropButton _dropButton;
        private bool _hasDrop;

        private void Awake()
        {
            _dropButton = FindObjectOfType<DropButton>();
            _dropButton.OnButtonPressed += OnButtonPressed;
            SpawnDrop();
        }

        private void OnButtonPressed()
        {
            if (!_hasDrop) return;
            StartCoroutine(Wait_c());
        }

        private IEnumerator Wait_c()
        {
            _hasDrop = false;
            yield return new WaitForSeconds(3f);
            SpawnDrop();
        }

        private void SpawnDrop()
        {
            Instantiate(drops[Random.Range(0, drops.Length)], spawnPoint);
            _hasDrop = true;
        }

        private void OnDestroy() => _dropButton.OnButtonPressed -= OnButtonPressed;
    }
}