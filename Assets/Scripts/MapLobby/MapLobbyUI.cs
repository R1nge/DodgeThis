using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace MapLobby
{
    public class MapLobbyUI : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private TextMeshProUGUI buttonText;

        public void UpdateUI(MinigameInstructionsSO instructions)
        {
            title.text = instructions.title;
            icon.sprite = instructions.image;
            description.text = instructions.description;
        }

        public void UpdateButton(bool state, bool isServer)
        {
            if (isServer)
            {
                buttonText.text = state ? "<color=green>Start</color>" : "<color=red>Start</color>";
                buttonText.transform.parent.GetComponent<Button>().interactable = state;
            }
            else
            {
                buttonText.text = state ? "<color=green>Ready</color>" : "<color=red>Not Ready</color>";
            }
        }
    }
}