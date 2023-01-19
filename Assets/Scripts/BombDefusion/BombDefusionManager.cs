using System.Collections.Generic;
using UnityEngine;

namespace BombDefusion
{
    public enum Colors : byte
    {
        Blue,
        Red,
        Green,
        Yellow
    }

    public class BombDefusionManager : MonoBehaviour
    {
        [SerializeField] private DefuseButton[] buttons;
        [SerializeField] private byte maxSequence;
        private List<Colors> _colorSequence = new();
        private List<Colors> _colorsClicked = new();
        private byte _lastIndex;

        private void Awake()
        {
            InvokeRepeating(nameof(Next), 2f, 2f);
        }

        private void Next()
        {
            if (_colorsClicked.Count != _colorSequence.Count || _colorSequence.Count >= maxSequence) return;
            var color = GetRandomColor();
            _colorSequence.Add(color);
            _colorsClicked.Add(color);
            Show();
        }

        private void Show()
        {
            
        }

        private Colors GetRandomColor() => (Colors)Random.Range(0, 4);
    }
}