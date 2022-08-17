using System;
using System.Collections;
using System.Collections.Generic;
using HelperScripts;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static HelperScripts.NatoAlphabetConverter;

namespace Menu
{
	public class ConsoleTextHandler : MonoBehaviour
	{
		[SerializeField] private bool forceNewLine = true;
		[SerializeField] private bool animateAllAtStart = true;
		[DoNotSerialize] private const bool DoAnimateByDefault = true;// change to false to disable animation by default
		
		[SerializeField] private TextMeshProUGUI mTextMeshPro;
		
		[SerializeField]public TypeWriterEffect typeWriterEffectScript;
		[DoNotSerialize]private   QueueManager _animationQueue; 
	
		private void OnValidate()
		{
			MyDebug.CheckIfReferenceExistsOrComponentExistsInGameObject(mTextMeshPro,this, this.gameObject);
			MyDebug.CheckIfReferenceExistsOrComponentExistsInGameObject(typeWriterEffectScript,this, this.gameObject);
		}

		private void Awake()
		{
			_animationQueue= gameObject.AddComponent<QueueManager>();
		}

		private void Start()
		{
			if (animateAllAtStart)
				_animationQueue.AddToQueue(typeWriterEffectScript.AnimateAll());
		}

		public void AddTextToConsole(string textToAdd, MessageType messageType = MessageType.None, bool doAnimate = DoAnimateByDefault)
		{
			if (forceNewLine) textToAdd = Environment.NewLine + textToAdd; //adding newline to the beginning;
			var textWithColor = TextManipulation.AddColorToText(textToAdd, messageType);
			
			if (doAnimate) _animationQueue.AddToQueue(typeWriterEffectScript.AddAnimatedText(textWithColor));
			else _animationQueue.AddToQueue(AddTextWithoutAnimation(textWithColor));
		}

		private IEnumerator AddTextWithoutAnimation(string textToAdd)
		{
			mTextMeshPro.text += textToAdd;
			mTextMeshPro.maxVisibleCharacters = mTextMeshPro.textInfo.characterCount;
			yield return null;
		}

		////testing function to make sure that the typewriter effect works
		// public void AddTextLoop(int iterations)
		// {
		// 	for (int i = 0; i <= iterations; i++)
		// 	{
		// 		AddTextToConsole(IntToLetters(i),MessageType.Info,true); 
		// 		AddTextToConsole(LettersToName(IntToLetters(i)),MessageType.Info,true);
		// 	}     
		// }
	}
}