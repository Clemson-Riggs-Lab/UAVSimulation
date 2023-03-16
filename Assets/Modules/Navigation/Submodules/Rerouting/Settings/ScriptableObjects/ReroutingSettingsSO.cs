using UnityEngine;
using UnityEngine.Serialization;

namespace Modules.Navigation.Submodules.Rerouting.Settings.ScriptableObjects
{ 
	[CreateAssetMenu(fileName = "ReroutingSettings", menuName = "Settings/ReroutingSettings")]
	public class ReroutingSettingsSO:ScriptableObject
	{
		
		[FormerlySerializedAs("oneClickReroute")]
		[Space(20)]
		
		
		[Space(20)]
		[Header("One Click Rerouting Settings")]
		public bool oneClickRerouteEnabled= true;
		public float oneClickRerouteFalsePositiveProbability = 0.1f;
		public float oneClickRerouteFalseNegativeProbability = 0.1f;
		public float probabilityOfUnsuccessfulOneClickReroute = 0.7f;
		public int oneClickRerouteRandomSeed = 1;
		public int oneClickRerouteFpFnRandomSeed = 1;
		
		[Space(20)]
		[Header("Logging Settings")]
		public bool logReroutingEvents = true;
		public bool logIfReroutingWasNeeded = true;
		public bool logIfNewRouteIsGoodOrBad = true;
		public bool logTimeOfPathStart= true;
		
		public bool logReroutingOptionsRequested = true;

		public bool logReroutingOptionPreview = true;
		public bool logIfPreviewedRouteIsGoodOrBad = true;
		public bool logOneClickReroutingRequested = true;
	}
}