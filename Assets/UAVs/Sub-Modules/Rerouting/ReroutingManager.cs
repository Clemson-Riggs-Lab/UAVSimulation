using System;
using ScriptableObjects.EventChannels;
using UnityEngine;

namespace UAVs.Sub_Modules.Rerouting
{
	public class ReroutingManager:MonoBehaviour
	{
		public GameObject reroutingButtonsContainer;
		public GameObject reroutingOptionsPanelsContainer;


		private void Start()
		{
			reroutingButtonsContainer.AddComponent<ReroutingButtonsContainerManager>();
		}
	}
}