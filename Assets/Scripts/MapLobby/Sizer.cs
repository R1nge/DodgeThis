using Unity.Netcode;
using UnityEngine;

namespace MapLobby
{
    public class Sizer : NetworkBehaviour
    {
        [SerializeField] private Vector3 position;
        public void Start()
        {
            transform.parent = GameObject.Find("PlayersList(Clone)").transform;
            var canvas = GetComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 1;
            var rectT = GetComponent<RectTransform>();
            rectT.pivot = new Vector2(0, 1);
            transform.localScale = Vector3.one;
            transform.localPosition = position;
        }
    }
}