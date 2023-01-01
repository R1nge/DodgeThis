using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Crane
{
    public class DropSpawner : NetworkBehaviour
    {
        [SerializeField] private GameObject[] drops;
        private GameObject _spawnPoint;
        private DropButton _dropButton;
        private bool _hasDrop;

        private void Start()
        {
            _dropButton = FindObjectOfType<DropButton>();
            _dropButton.OnButtonPressed += OnButtonPressed;
        }

        public override void OnNetworkSpawn()
        {
            StartCoroutine(Wait_c());
            _spawnPoint = GameObject.Find("CranePlayer(Clone)/SpawnPoint(Clone)");
        }

        private void OnButtonPressed()
        {
            if (!_hasDrop) return;
            StartCoroutine(Wait_c());
        }

        private IEnumerator Wait_c()
        {
            _hasDrop = false;
            _spawnPoint = GameObject.Find("CranePlayer(Clone)/SpawnPoint(Clone)");
            yield return new WaitForSeconds(3f);
            _spawnPoint = GameObject.Find("CranePlayer(Clone)/SpawnPoint(Clone)");
            SpawnDropServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnDropServerRpc()
        {
            var inst = Instantiate(drops[Random.Range(0, drops.Length)], _spawnPoint.transform);
            inst.GetComponent<NetworkObject>().Spawn(true);
            inst.transform.parent = _spawnPoint.transform;
            _hasDrop = true;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _dropButton.OnButtonPressed -= OnButtonPressed;
        }
    }
}