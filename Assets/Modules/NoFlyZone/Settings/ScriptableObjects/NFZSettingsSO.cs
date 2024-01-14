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
		public bool animateNFZGrowOnStart=false;
		public float nfzGrowthAnimationDuration=0f;
		public string nfzCountdownText = "NFZ";
		public bool nfzAddCountdownCounterToText = false;
		public bool blinkNFZOnCountdownCounter = false;
		public float nfzBlinkInterval = 0f;
		
		
		[Space(20)]
		[Header("Logging Settings")]
		public bool logNFZCollisions = true;
	}
}