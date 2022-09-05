using HelperScripts;
using TMPro;
using UnityEngine;

namespace Menu
{
	public class KeyValuePairController : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI keyText;
		[SerializeField] private TextMeshProUGUI valueText;

		private void OnValidate()
		{
			AssertionHelper.AssertComponentReferencedInEditor(keyText, this,this.gameObject);
			AssertionHelper.AssertComponentReferencedInEditor(valueText,this, this.gameObject);
		}

		public void SetKeyText(string text, string color)
		{
			keyText.text = TextManipulation.AddColorToText(text, color);
		}

		public void SetValueText(string text, string color)
		{
			valueText.text = TextManipulation.AddColorToText(text, color);
		}

		public string GetKeyText()
		{
			keyText.ForceMeshUpdate(); //since we often use this at start, the mesh wouldn't be initialized and thus GetParsedText would return an empty string. ForceMeshUpdate Mitigates this issue
			return keyText.GetParsedText();
		}

		public string GetValueText()
		{
			keyText.ForceMeshUpdate();
			return valueText.GetParsedText();
		}
	}
}