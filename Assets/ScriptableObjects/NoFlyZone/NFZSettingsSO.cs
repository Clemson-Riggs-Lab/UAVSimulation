using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace ScriptableObjects.NoFlyZone
{
	[CreateAssetMenu(fileName = "NFZSettings", menuName = "Settings/NFZ Settings")]
	public class NFZSettingsSO:ScriptableObject
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public NFZRecordsSource nfzRecordsSource =  NFZRecordsSource.FromDefaultRecords;
		
		[Space(20)]
		public bool animateNFZGrowOnStart=true;
		public float nfzGrowthAnimationDuration=3f;
		public string nfzCountdownText = "No Fly Zone Will Be Active In";
		public bool nfzAddCountdownCounterToText = true;
		public bool blinkNFZOnCountdownCounter = true;
		public float nfzBlinkInterval = 0.5f;
		
		
		public enum NFZRecordsSource
		{
			Disabled,
			FromFile,
			FromDefaultRecords,
		}
	}
}