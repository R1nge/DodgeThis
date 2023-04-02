using TMPro;
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
            if(!_gameSelectionManager.CanSelect()) return;
            _index = index;
            _gameSelectionManager.SelectServerRpc(_index, _gameSelectionManager.GetSelectedAmount());
            _gameSelectionManager.Select();
            gameObject.SetActive(false);
            _index--;
        }
    }
}