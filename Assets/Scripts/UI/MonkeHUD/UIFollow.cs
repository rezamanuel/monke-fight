
using UnityEngine;

namespace Monke.UI
{
    public class UIFollow : MonoBehaviour
    {

        public Transform Follow;

        private Camera MainCamera;

        [SerializeField] Vector3 Displacement;

        // Start is called before the first frame update
        void Start()
        {
            MainCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            var screenPos = MainCamera.WorldToScreenPoint(Follow.position);

            transform.position = screenPos + Displacement;
        }
    }
}