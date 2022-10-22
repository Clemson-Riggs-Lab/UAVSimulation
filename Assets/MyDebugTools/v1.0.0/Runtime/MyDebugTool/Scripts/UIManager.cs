using UnityEngine;

namespace MyDebugTools
{
    public class UIManager : PersistentSingleton<UIManager>
    {
        [SerializeField] private GameObject myInGameDebugConsole;
        [SerializeField] private GameObject myRuntimeInspector;

        protected override void Awake()
        {
            //myInGameDebugConsole.SetActive(false);
            //myRuntimeInspector.SetActive(false);

            base.Awake();
        }

        public void DebugToolToggle()
        {
            if (myInGameDebugConsole.activeSelf == true)
            {
                myInGameDebugConsole.SetActive(false);
                myRuntimeInspector.SetActive(false);
            }
            else
            {
                myInGameDebugConsole.SetActive(true);
                myRuntimeInspector.SetActive(true);
            }
        }
    }
}

