using HelperScripts;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.Serialization;

namespace Modules.NoFlyZone.Settings.ScriptableObjects
{
	[CreateAssetMenu(fileName = "NFZSettings", menuName = "Settings/NFZ Settings")]
	public class NFZSettingsSO:ScriptableObject
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public Enums.InputRecordsSource nfzRecordsSource =  Enums.InputRecordsSource.FromInputFile;
		
		[FormerlySerializedAs("defaultRatioOfHeadToNFZ")]
		[Space(20)]
		[Header("UAV Settings")]
		public float RatioOfHeadToNFZ = 0.1f;
		
		[Space(20)]
		[Header("NFZ Settings")]
		public bool animateNFZGrowOnStart=true;
		public float nfzGrowthAnimationDuration=3f;
		public string nfzCountdownText = "No Fly Zone will be active in";
		public bool nfzAddCountdownCounterToText = true;
		public bool blinkNFZOnCountdownCounter = true;
		public float nfzBlinkInterval = 0.5f;
		
		
		[Space(20)]
		[Header("Logging Settings")]
		public bool logNFZCollisions = true;
	}
}