using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelperScripts
{
	public class QueueManager : MonoBehaviour
	{
		private readonly Queue<IEnumerator> _coroutineQueue = new Queue<IEnumerator>();
		private Coroutine _coroutine;

		public void AddToQueue(IEnumerator coroutine)
		{
			_coroutineQueue.Enqueue(coroutine);
			if (_coroutineQueue.Count == 1) //no previous elements in queue
				_coroutine = StartCoroutine(CoroutineCoordinator());
			
		}

		private IEnumerator CoroutineCoordinator()
		{
			while (true)
			{
				while (_coroutineQueue.Count > 0)
				{
					yield return StartCoroutine(_coroutineQueue.Peek());
					_coroutineQueue.Dequeue();
				}
				
				if (_coroutineQueue.Count == 0)
					yield break;
			}
		}

		public void ClearQueue()
		{
			_coroutineQueue.Clear();
			if (_coroutine != null)
				StopCoroutine(_coroutine);
		}

		private void OnDisable()
		{
			ClearQueue();
		}
	}
}