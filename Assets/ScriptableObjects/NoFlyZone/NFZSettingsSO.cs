using HelperScripts;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace ScriptableObjects.NoFlyZone
{
	[CreateAssetMenu(fileName = "NFZSettings", menuName = "Settings/NFZ Settings")]
	public class NFZSettingsSO:ScriptableObject
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public Enums.InputRecordsSource nfzRecordsSource =  Enums.InputRecordsSource.FromInputFile;
		
		[Space(20)]
		public bool animateNFZGrowOnStart=true;
		public float nfzGrowthAnimationDuration=3f;
		public string nfzCountdownText = "No Fly Zone will be active in";
		public bool nfzAddCountdownCounterToText = true;
		public bool blinkNFZOnCountdownCounter = true;
		public float nfzBlinkInterval = 0.5f;
		
	}
}