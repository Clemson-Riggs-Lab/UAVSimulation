using System;
using Helper_Scripts;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Menu
{
	public class ConsoleTextHandler : MonoBehaviour
	{
		public TypeWriterEffect typeWriterEffectScript;

		private void OnValidate()
		{
			MyDebug.AssertComponentReferencedInEditor(typeWriterEffectScript, this.gameObject);
		}

		public void AddTextToConsole(string text, MessageType messageType = MessageType.None,
			bool enableAnimation = true)
		{
			var textWithColor = TextManipulation.AddColorToText(text, messageType);
			typeWriterEffectScript.QueueTextAddition(textWithColor, enableAnimation);
		}
	}
}