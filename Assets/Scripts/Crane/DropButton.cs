using System;
using UnityEngine;

namespace Crane
{
    public class DropButton : MonoBehaviour
    {
        public event Action OnButtonPressed;
        private void OnMouseDown() => OnButtonPressed?.Invoke();
    }
}