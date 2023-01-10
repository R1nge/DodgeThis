using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace GameSelection
{
    public class GameSlotUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title, description;
        [SerializeField] private RawImage preview, icon;
        [SerializeField] private Button select;
        private int _index;
        private GameSelectionManager _gameSelectionManager;

        private void Awake()
        {
            _gameSelectionManager = FindObjectOfType<GameSelectionManager>();
        }
        
        public void UpdateSlotInfo(GamesUI ui)
        {
            title.text = ui.title;
            //description.text = ui.description;
            preview.texture = ui.preview.texture;
            icon.texture = ui.icon.texture;
    
        }
        
        public void AddButtonListener(int index)
        {
            select.onClick.RemoveAllListeners();
            select.onClick.AddListener(() => { Select(index); });
        }
        
        private void Select(int index)
        {
            _index = index;
            _gameSelectionManager.SelectServerRpc(_index);
            gameObject.SetActive(false);
            _index--;
        }
        //
        // [ClientRpc]
        // private void SelectClientRpc(int index)
        // {
        //     _index = index;
        //     GameSelectionSingleton.Instance.SelectGameServerRpc(_index);
        //     gameObject.SetActive(false);
        //     _index--;
        // }
    }
}