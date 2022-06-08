using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Helper_Scripts;
using TMPro;
using UnityEngine;
using static NatoAlphabetConverter;

namespace Menu
{
	//TODO: Split this into two classes (one that handles the typewriter effect, and one that handles the queue. The queue class should be generic so it can be used for other classes, e.g., UAV routing, etc.)
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




		/*public void QueueTextAddition(string textToAdd, bool animateText = true)

		{
			var addTextMethod = AddText(textToAdd, animateText);
			if (_runningCoroutine == null)
			{
				_runningCoroutine = addTextMethod;
				StartCoroutine(_runningCoroutine);
			}
			else
				_taskQueue.Enqueue(addTextMethod);
		}*/
		
		public IEnumerator AddAnimatedText(string textToAdd)
		{
			var startAnimationPosition = mTextMeshPro.textInfo.characterCount; //get length
			mTextMeshPro.text += textToAdd; //append
			mTextMeshPro.ForceMeshUpdate(); //update mesh
			var endAnimationPosition = mTextMeshPro.textInfo.characterCount; //get new length

			var totalCharacters = endAnimationPosition - startAnimationPosition;
			var animationDuration = totalCharacters * durationOfSingleCharacterAnimation;

			yield return StartCoroutine(AnimateText(startAnimationPosition, endAnimationPosition));
			yield return new WaitForSeconds(animationDuration);
		}

		// private bool _hasARunningTask = false;
		// IEnumerator _currentTask = null;
		// public void QueueAction(IEnumerator methodToQueue)
		// 		{
		// 			//_taskQueue.Enqueue(methodToQueue);
		// 			_queueManager.Enqueue(methodToQueue);
		// 			if (_hasARunningTask == false)
		// 			{
		// 				StartCoroutine(StartQueue());
		// 			}
		//
		// 		}
		// private IEnumerator StartQueue()
		// {
		// 	_hasARunningTask = true;
		// 	
		// 	while (_taskQueue.Count > 0)
		// 	{
		// 		Debug.Log("Visible Count: ");
		// 		 _currentTask = _taskQueue.Dequeue();
		// 		yield return  StartCoroutine(_currentTask);
		// 		if(_taskQueue.Count == 0)_hasARunningTask = false;
		// 	}
		// 	
		// }

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