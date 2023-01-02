using System.Collections;
using UnityEngine;

namespace NumbersFloor
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private Materials materials;
        private int _currentNumber;
        private BoxCollider _boxCollider;
        private MeshRenderer _meshRenderer;
        

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _boxCollider = GetComponent<BoxCollider>();
        }

        private void Start()
        {
            _currentNumber = Random.Range(3, 9);
            StartCoroutine(DecreaseNumber());
        }

        private IEnumerator DecreaseNumber()
        {
            while (enabled)
            {
                yield return new WaitForSeconds(1);
                _currentNumber--;

                if (_currentNumber == 9)
                {
                    _meshRenderer.material = materials.GetMaterial(9);
                    _meshRenderer.material.color = materials.Purple;
                }
                else if (_currentNumber == 8)
                {
                    _meshRenderer.material = materials.GetMaterial(8);
                    _meshRenderer.material.color = materials.DarkBlue;
                }
                else if (_currentNumber == 7)
                {
                    _meshRenderer.material = materials.GetMaterial(7);
                    _meshRenderer.material.color = materials.Blue;
                }
                else if (_currentNumber == 6)
                {
                    _meshRenderer.material = materials.GetMaterial(6);
                    _meshRenderer.material.color = materials.LightBlue;
                }
                else if (_currentNumber == 5)
                {
                    _meshRenderer.material = materials.GetMaterial(5);
                    _meshRenderer.material.color = materials.Emerald;
                }
                else if (_currentNumber == 4)
                {
                    _meshRenderer.material = materials.GetMaterial(4);
                    _meshRenderer.material.color = materials.LightGreen;
                }
                else if (_currentNumber == 3)
                {
                    _meshRenderer.material = materials.GetMaterial(3);
                    _meshRenderer.material.color = materials.YellowGreen;
                }
                else if (_currentNumber == 2)
                {
                    _meshRenderer.material = materials.GetMaterial(2);
                    _meshRenderer.material.color = materials.Orange;
                }
                else if (_currentNumber == 1)
                {
                    _meshRenderer.material = materials.GetMaterial(1);
                    _meshRenderer.material.color = materials.DarkOrange;
                }
                else if (_currentNumber == 0)
                {
                    _meshRenderer.material = materials.GetMaterial(0);
                    _meshRenderer.material.color = materials.Red;
                }

                if (_currentNumber <= 0)
                {
                    _boxCollider.enabled = false;
                    yield return new WaitForSeconds(1);
                    _boxCollider.enabled = true;
                    _currentNumber = Random.Range(3, 9);
                }
            }
        }
    }
}