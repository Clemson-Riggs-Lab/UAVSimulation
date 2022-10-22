using UnityEngine;

namespace MyRuntimeInspect
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject runtimeHierarchy;
        [SerializeField] private GameObject runtimeInspector;

        private void Start()
        {
            runtimeHierarchy.SetActive(false);
            runtimeInspector.SetActive(false);
        }

        public void ShowIns()
        {
            runtimeInspector.SetActive(true);
        }

        public void HideIns()
        {
            runtimeInspector.SetActive(false);
        }

        public void ShowHie()
        {
            runtimeHierarchy.SetActive(true);
        }

        public void HideHie()
        {
            runtimeHierarchy.SetActive(false);
        }
    }
}

