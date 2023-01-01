using System;
using UnityEngine;

public class ResetButton : MonoBehaviour
{
    public event Action OnButtonPressed;
    private void OnMouseDown() => OnButtonPressed?.Invoke();
}