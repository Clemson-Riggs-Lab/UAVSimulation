using System;
using System.Collections;
using System.Collections.Generic;
using Helper_Scripts;
using TMPro;
using UnityEngine;

namespace Menu
{
	public class TypeWriterEffect : MonoBehaviour
	{
		[SerializeField] public float durationOfSingleCharacterAnimation = 0.02f;
		[SerializeField] public bool startDirectly = true;
		[SerializeField] public bool forceNewLine = true;

		[SerializeField] public TextMeshProUGUI mTextMeshPro;

		//animation queue variables
		private IEnumerator _runningCoroutine = null;
		private Queue<IEnumerator> _coroutineQueue = new Queue<IEnumerator>();

		private void OnValidate()
		{
			MyDebug.AssertComponentReferencedInEditor(mTextMeshPro, this.gameObject);
		}

		void Start()
		{
			mTextMeshPro.ForceMeshUpdate(); //since we are calling in the start, this forces to render the text mesh. Otherwise we wouldnt be able to get the attributes since it hasnt laoded yet.
			mTextMeshPro.maxVisibleCharacters = 0; //making text invisible.

			if (startDirectly)
			{
				AnimateAll();
			}
		}

		public void AnimateAll()

		{
			var totalCharacters = mTextMeshPro.textInfo.characterCount;
			StartCoroutine(AnimateText(0, totalCharacters));
		}

		// testing function to make sure that the typewriter effect works
		/*public void AddTextLoop(int iterations)
	{
		for (int i = 0; i <= iterations; i++)
		{
		    QueueTextAddition(IntToLetters(i)); 
		    QueueTextAddition(LettersToName(IntToLetters(i)));
		}     
	}*/


		public void QueueTextAddition(string textToAdd, bool animateText = true)

		{
			var addTextMethod = AddText(textToAdd, animateText);
			if (_runningCoroutine == null)
			{
				_runningCoroutine = addTextMethod;
				StartCoroutine(_runningCoroutine);
			}
			else
				_coroutineQueue.Enqueue(addTextMethod);
		}


		private IEnumerator AddText(string textToAdd, bool animateText)
		{
			if (forceNewLine) textToAdd = Environment.NewLine + textToAdd; //adding newline to the beginning;
			if (animateText)
			{
				var startAnimationPosition = mTextMeshPro.textInfo.characterCount; //get length
				mTextMeshPro.text += textToAdd; //append
				var endAnimationPosition = mTextMeshPro.textInfo.characterCount; //get new length

				var totalCharacters = endAnimationPosition - startAnimationPosition;
				var animationDuration = totalCharacters * durationOfSingleCharacterAnimation;

				StartCoroutine(AnimateText(startAnimationPosition, endAnimationPosition));
				yield return new WaitForSeconds(animationDuration);
			}

			else
			{
				mTextMeshPro.text += textToAdd;
				mTextMeshPro.maxVisibleCharacters = mTextMeshPro.textInfo.characterCount;
			}

			CheckAndUpdateQueue();
		}

		private void CheckAndUpdateQueue()
		{
			_runningCoroutine = null;
			if (_coroutineQueue.Count > 0)
			{
				_runningCoroutine = _coroutineQueue.Dequeue();
				StartCoroutine(_runningCoroutine);
			}
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