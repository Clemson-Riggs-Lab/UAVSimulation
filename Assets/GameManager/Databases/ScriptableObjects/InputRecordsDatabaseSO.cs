using System.Collections.Generic;
using IOHandlers;
using Modules.Prompts;
using UnityEngine;

namespace Databases.ScriptableObjects
{
	[CreateAssetMenu(fileName = "InputRecords", menuName = "Database/InputRecords", order = 0)]
	public class InputRecordsDatabaseSO: ScriptableObject
	{
		public List<WayPointRecord> WayPointsRecords { get; set; } = new List<WayPointRecord>();
		public List<UavDynamicNavigationWorkloadRecord> UavDynamicNavigationWorkloadRecords { get; set; } = new List<UavDynamicNavigationWorkloadRecord>();
		public List<UavDynamicTargetDetectionWorkloadRecord> UavDynamicTargetDetectionWorkloadRecords { get; set; } = new List<UavDynamicTargetDetectionWorkloadRecord>();
		public List<UavFuelLeaksRecord> FuelLeaksRecord { get; set; }= new List<UavFuelLeaksRecord>();
		public List<Prompt> Prompts { get; set; } = new List<Prompt>();
		public List<NFZRecord> NFZRecords { get; set; } = new List<NFZRecord>();
	}
}