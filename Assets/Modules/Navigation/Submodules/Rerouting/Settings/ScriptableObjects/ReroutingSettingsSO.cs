using UnityEngine;

namespace Modules.Navigation.Submodules.Rerouting.Settings.ScriptableObjects
{ 
	[CreateAssetMenu(fileName = "ReroutingSettings", menuName = "Settings/ReroutingSettings")]
	public class ReroutingSettingsSO:ScriptableObject
	{
		[Space(20)]
		[Header("Logging Settings")]
		
		public bool logReroutingEvents = true;
		public bool logIfReroutingWasNeeded = true;
		public bool logIfNewRouteIsGoodOrBad = true;
		public bool logTimeOfPathStart= true;
		
		public bool logReroutingOptionsRequested = true;

		public bool logReroutingOptionPreview = true;
		public bool logIfPreviewedRouteIsGoodOrBad = true;
		
	}
}