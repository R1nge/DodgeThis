using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace BallArena
{
    public class BallSpawner : NetworkBehaviour
    {
        [SerializeField] private GameObject ball;
        [SerializeField] private Vector3 spawnPosition;
        [SerializeField] private float firstDelay, delay;
        private GameState _gameState;

        private void Awake()
        {
            _gameState = FindObjectOfType<GameState>();
            _gameState.OnGameStarted += OnGameStarted;
        }

        private void OnGameStarted()
        {
            if (!IsServer) return;
            StartCoroutine(WaitFirst());
        }

        private IEnumerator WaitFirst()
        {
            yield return new WaitForSeconds(firstDelay);
            Spawn();
        }

        private IEnumerator Wait()
        {
            yield return new WaitForSeconds(delay);
            Spawn();
        }

        private void Spawn()
        {
            if (!IsServer) return;
            var inst = Instantiate(ball, spawnPosition, Quaternion.identity);
            inst.GetComponent<NetworkObject>().Spawn(true);
            StartCoroutine(Wait());
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (_gameState == null) return;
            _gameState.OnGameStarted -= OnGameStarted;
        }
    }
}