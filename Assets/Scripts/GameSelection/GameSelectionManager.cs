using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameSelection
{
    public class GameSelectionManager : NetworkBehaviour
    {
        [SerializeField] private Transform parent;
        [SerializeField] private GameObject slot;
        private List<GameObject> _slots;

        private void Awake()
        {
            _slots = new List<GameObject>();
        }

        //On mouse down select game
        //Add to GameSelectionSingleton  Selected games

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                for (int i = 0; i < GameSelectionSingleton.Instance.GetGames().Count; i++)
                {
                    SpawnMinigamesUI(i);
                }
            }
        }

        private void SpawnMinigamesUI(int index)
        {
            SpawnMinigamesUIClientRpc(index);
        }

        [ClientRpc]
        private void SpawnMinigamesUIClientRpc(int index)
        {
            var slotInst = Instantiate(slot, parent);
            var slotUI = slotInst.GetComponent<GameSlotUI>();
            slotUI.UpdateSlotInfo(GameSelectionSingleton.Instance.GetGamesUI(index));
            slotUI.AddButtonListener(index);
            _slots.Add(slotInst);
        }

        [ServerRpc(RequireOwnership = false)]
        public void SelectServerRpc(int index)
        {
            print("Selected");
            SelectClientRpc(index);
            GameSelectionSingleton.Instance.SelectGameServerRpc(index);
        }

        [ClientRpc]
        private void SelectClientRpc(int index)
        {
            print("Selected");
            _slots[index].gameObject.SetActive(false);
        }

        public void StartGame()
        {
            NetworkManager.Singleton.SceneManager.LoadScene("SelectRandomGame", LoadSceneMode.Single);
        }
    }
}