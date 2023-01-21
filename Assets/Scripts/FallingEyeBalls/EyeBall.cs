using UnityEngine;

namespace FallingEyeBalls
{
    public class EyeBall : MonoBehaviour
    {
        [SerializeField] protected EyeColors currentColor;

        public EyeColors GetColor() => currentColor;
    }
}