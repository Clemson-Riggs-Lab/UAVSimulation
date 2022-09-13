using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockingPanelController : MonoBehaviour
{
	public TextMeshProUGUI tmpro;
	public Button closeButton;

	private const string LoadingText = "Loading... \n Trial will start in  \n";

	private const string ClosingText = "The Experiment is over, \n Thank you for participating! \n";


	public void LoadingView(float duration)
	{
		// make the panel visible by setting scale to 1
		transform.localScale = Vector3.one;
		
		//start a coroutine to animate the text every 0.5 seconds
		StartCoroutine(AnimateText(LoadingText,duration));
		
		
	}

	private IEnumerator AnimateText(string loadingText, float SimulationStartTime)
	{ 
		
		tmpro.text = loadingText;
		while(Time.time < SimulationStartTime)
		{
			tmpro.text = loadingText + "  "+ (SimulationStartTime - Time.time).ToString("F1");
			yield return new WaitForSeconds(0.1f);
		}
		
		// hide the panel by setting scale to 0
		transform.localScale = Vector3.zero;
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
}
