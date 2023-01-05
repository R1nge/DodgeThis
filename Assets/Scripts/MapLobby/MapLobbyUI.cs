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
                if (state)
                {
                    buttonText.text = "<color=#25be5b>Start</color>";
                }
                else
                {
                    buttonText.text = "<color=#C74229>Start</color>";
                }

                buttonText.transform.parent.GetComponent<Button>().interactable = state;
            }
            else
            {
                if (state)
                {
                    buttonText.text = "<color=#25be5b>Ready</color>";
                }
                else
                {
                    buttonText.text = "<color=#C74229>Not Ready</color>";
                }
            }
        }
    }
}