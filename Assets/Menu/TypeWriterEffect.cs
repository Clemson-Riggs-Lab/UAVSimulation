using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using HelperScripts;
using TMPro;
using UnityEngine;
using static HelperScripts.NatoAlphabetConverter;

namespace Menu
{ 
	public class TypeWriterEffect : MonoBehaviour
	{
		[SerializeField] public float durationOfSingleCharacterAnimation = 0.02f;
		[SerializeField] public TextMeshProUGUI mTextMeshPro;
		[SerializeField] public ConsoleTextHandler consoleTextHandler;
		
		private void OnValidate()
		{
			MyDebug.CheckIfReferenceExistsOrComponentExistsInGameObject(mTextMeshPro,this, this.gameObject);
			MyDebug.CheckIfReferenceExistsOrComponentExistsInGameObject(consoleTextHandler,this, this.gameObject);
		}
		
		public IEnumerator AnimateAll()

		{
			mTextMeshPro.ForceMeshUpdate();
			var totalCharacters = mTextMeshPro.textInfo.characterCount;
			yield return StartCoroutine(AnimateText(0, totalCharacters));
			yield return null;
		}

		public IEnumerator AddAnimatedText(string textToAdd)
		{

			// getting the start and end positions of the animation (which characters to animate)
			var startAnimationPosition = mTextMeshPro.textInfo.characterCount;
			mTextMeshPro.text += textToAdd; //append the text
			mTextMeshPro.ForceMeshUpdate(); //update mesh
			var endAnimationPosition = mTextMeshPro.textInfo.characterCount;

			//getting animation duration
			var totalCharacters = endAnimationPosition - startAnimationPosition;
			var animationDuration = totalCharacters * durationOfSingleCharacterAnimation;
			
			var animateTextCoroutine = AnimateText(startAnimationPosition, endAnimationPosition);
			StartCoroutine(animateTextCoroutine);
			
			yield return new WaitForSeconds(animationDuration);
		}
		
		private IEnumerator AnimateText(int startCharPos, int endCharPos)
		{
			int visibleCount = startCharPos;
			var delayBetweenLetters = new WaitForSeconds(durationOfSingleCharacterAnimation);

			while (visibleCount < endCharPos)
			{
				visibleCount++;
				mTextMeshPro.maxVisibleCharacters = visibleCount;
				yield return delayBetweenLetters;
			}
		}
	}
}