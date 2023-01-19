using System;
using System.Collections;
using UnityEngine;

namespace BombDefusion
{
    public class DefuseButton : MonoBehaviour
    {
        [SerializeField] private Colors currentColor;
        [SerializeField] private Material defaultColor, lightUpColor;
        private MeshRenderer _meshRenderer;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        public Colors GetColor() => currentColor;

        public void LightUp()
        {
            StartCoroutine(LightUp_c());
        }

        private IEnumerator LightUp_c()
        {
            yield return new WaitForSeconds(.2f);
            _meshRenderer.material = lightUpColor;
            yield return new WaitForSeconds(1);
            _meshRenderer.material = defaultColor;
        }
    }
}