using System;
using UnityEngine;

namespace DefaultNamespace
{
	public class CanvasManager : MonoBehaviour
	{
		public static CanvasManager Instance { get; private set; }

		public GameObject fuelAndHealthPanel;
		public GameObject TargetDetectionCamerasContainerPanel;

		private void Awake()
		{
			// If there is an instance, and it's not me, delete myself.

			if (Instance != null && Instance != this)
			{
				Destroy(this);
			}
			else
			{
				Instance = this;
			}

		}

		private void Start()
		{
			foreach(Transform child in fuelAndHealthPanel.transform)
			{
				Destroy(child.gameObject);
			}
			foreach(Transform child in TargetDetectionCamerasContainerPanel.transform)
			{
				Destroy(child.gameObject);
			}
		}
	}
}