using System;
using UnityEngine;

namespace Crane
{
    public class ResetButton : MonoBehaviour
    {
        public event Action OnButtonPressed;
        private void OnMouseDown() => OnButtonPressed?.Invoke();
    }
}