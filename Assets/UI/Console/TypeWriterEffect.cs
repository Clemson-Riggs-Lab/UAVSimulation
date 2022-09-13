using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using HelperScripts;
using ScriptableObjects.Databases;
using TMPro;
using UnityEngine;
using static HelperScripts.NatoAlphabetConverter;

namespace Menu
{ 
	public class TypeWriterEffect : MonoBehaviour
	{
		[SerializeField] public TextMeshProUGUI mTextMeshPro;
		[SerializeField] public SettingsDatabaseSO settingsDatabase;
		private float DurationOfSingleCharacterAnimation => 1f/settingsDatabase.promptSettings.textAnimationSpeed;
		private Coroutine _coroutine;
		

		public IEnumerator AnimateAll()

		{
			mTextMeshPro.ForceMeshUpdate();
			var totalCharacters = mTextMeshPro.textInfo.characterCount;
			yield return StartCoroutine(AnimateText(0, totalCharacters,DurationOfSingleCharacterAnimation));
			yield return null;
		}

		public IEnumerator AddAnimatedText(string textToAdd,float duration=0 )
		{
			if(duration==0) //if no value provided, then default to the value set in the settings database
			{
				duration = DurationOfSingleCharacterAnimation;
			}
			
			// getting the start and end positions of the animation (which characters to animate)
			var startAnimationPosition = mTextMeshPro.textInfo.characterCount;
			mTextMeshPro.text += textToAdd; //append the text
			mTextMeshPro.ForceMeshUpdate(); //update mesh
			var endAnimationPosition = mTextMeshPro.textInfo.characterCount;

			//getting animation duration
			var totalCharacters = endAnimationPosition - startAnimationPosition;
			var animationDuration = totalCharacters * duration;
			
			var animateTextCoroutine = AnimateText(startAnimationPosition, endAnimationPosition,duration);
			_coroutine=StartCoroutine(animateTextCoroutine);
			yield return new WaitForSeconds(animationDuration);
		}
		
		private IEnumerator AnimateText(int startCharPos, int endCharPos,float durationOfSingleCharAnimation)
		{
			int visibleCount = startCharPos;
			var delayBetweenLetters = new WaitForSeconds(durationOfSingleCharAnimation);

			while (visibleCount < endCharPos)
			{
				visibleCount++;
				mTextMeshPro.maxVisibleCharacters = visibleCount;
				yield return delayBetweenLetters;
			}
		}

		private void OnDisable()
		{
			if(_coroutine!=null)
				StopCoroutine(_coroutine);
		}
	}
}