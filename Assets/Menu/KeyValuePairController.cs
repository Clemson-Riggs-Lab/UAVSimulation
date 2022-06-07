using System;
using Helper_Scripts;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Menu
{
	public class KeyValuePairController : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI keyText;
		[SerializeField] private TextMeshProUGUI valueText;

		private void OnValidate()
		{
			MyDebug.AssertComponentReferencedInEditor(keyText, this.gameObject);
			MyDebug.AssertComponentReferencedInEditor(valueText, this.gameObject);
		}

		public void SetKeyText(string text, MessageType messageType)
		{
			keyText.text = TextManipulation.AddColorToText(text, messageType);
		}

		public void SetValueText(string text, MessageType messageType)
		{
			valueText.text = TextManipulation.AddColorToText(text, messageType);
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