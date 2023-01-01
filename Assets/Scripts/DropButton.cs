using System;
using UnityEngine;

public class DropButton : MonoBehaviour
{
    public event Action OnButtonPressed;
    private void OnMouseDown() => OnButtonPressed?.Invoke();
}