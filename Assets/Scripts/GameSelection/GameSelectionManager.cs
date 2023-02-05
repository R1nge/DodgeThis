using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameSelection
{
    public class GameSelectionManager : NetworkBehaviour
    {
        [SerializeField] private Transform parent;
        [SerializeField] private GameObject slot;
        [SerializeField] private int maxSelectedGames;
        [SerializeField] private TextMeshProUGUI gameSelectLeft;
        private List<GameObject> _slots;
        private int _selectedGames;

        private void Awake()
        {
            _slots = new List<GameObject>();
            gameSelectLeft.text = "Left: " + (maxSelectedGames - _selectedGames);
        }

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
            slotUI.UpdateSlotInfo(GameSelectionSingleton.Instance.GetGamesUI()[index]);
            slotUI.AddButtonListener(index);
            _slots.Add(slotInst);
        }

        [ServerRpc(RequireOwnership = false)]
        public void SelectServerRpc(int index, int selected)
        {
            if (selected >= maxSelectedGames) return;
            SelectClientRpc(index, selected);
            GameSelectionSingleton.Instance.SelectGame(index);
        }

        [ClientRpc]
        private void SelectClientRpc(int index, int selected)
        {
            if (selected >= maxSelectedGames) return;
            _slots[index].gameObject.SetActive(false);
            GameSelectionSingleton.Instance.SelectGame(index);
        }

        public void Select()
        {
            _selectedGames++;
            gameSelectLeft.text = "Left: " + (maxSelectedGames - _selectedGames);
        }

        public bool CanSelect() => _selectedGames < maxSelectedGames;

        public int GetSelectedAmount() => _selectedGames;

        public void StartGame()
        {
            NetworkManager.Singleton.SceneManager.LoadScene("SelectRandomGame", LoadSceneMode.Single);
        }
    }
}