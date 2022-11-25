using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class BlockingPanelController : MonoBehaviour
	{
		public TextMeshProUGUI tmpro;
		public Button closeButton;

		private const string LoadingText = "Loading... \n Trial will start in  \n";

		private const string PauseText = "The Experiment is Paused, \n Please wait for the conductor's instructions. \n";
		
		private const string ClosingText = "The Experiment is over, \n Thank you for participating! \n";


		public void ShowView()
		{
            // make the panel visible by setting scale to 1
            transform.localScale = Vector3.one;
        }

		public void HideView()
		{
			// hide the panel by setting scale to 0
			transform.localScale = Vector3.zero;
			closeButton.transform.localScale = Vector3.zero;
		}
		
		public void LoadingView(float duration)
		{
			//start a coroutine to animate the text every 0.5 seconds
			StartCoroutine(AnimateText(LoadingText,duration));
		}

		private IEnumerator AnimateText(string loadingText, float simulationStartTime)
		{ 
		
			tmpro.text = loadingText;
			while(Time.time < simulationStartTime)
			{
				tmpro.text = loadingText + "  "+ (simulationStartTime - Time.time).ToString("F1");
				yield return new WaitForSeconds(0.1f);
			}
		
			HideView();
		}
	
		public void ClosingView()
		{
			// make the panel visible by setting scale to 1
			transform.localScale = Vector3.one;
			tmpro.text = ClosingText;
		
			//make the close button visible
			closeButton.transform.localScale = Vector3.one;
			//wire up the close button
			closeButton.onClick.AddListener(() => Application.Quit());
		}
		
		
		public void PauseView(bool pause)
		{
			if (pause)
			{
				// make the panel visible by setting scale to 1
				transform.localScale = Vector3.one;
				tmpro.text = PauseText;

				//make the close button visible
				closeButton.transform.localScale = Vector3.one;
				//wire up the close button
				closeButton.onClick.AddListener(HideView);
			}
			else
			{
				HideView();
			}
		}
	}
}
