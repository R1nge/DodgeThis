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

        [ServerRpc(RequireOwnership = false)]
        public void UpdateSlotInfoServerRpc(GamesUI ui)
        {
            title.text = ui.title;
            //description.text = ui.description;
            preview.texture = ui.preview.texture;
            icon.texture = ui.icon.texture;
            UpdateSlotInfoClientRpc(ui);
        }

        [ClientRpc]
        private void UpdateSlotInfoClientRpc(GamesUI ui)
        {
            title.text = ui.title;
            //description.text = ui.description;
            preview.texture = ui.preview.texture;
            icon.texture = ui.icon.texture;
        }
    }
}