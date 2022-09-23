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
		public List<UavRecord> UavsRecords { get; set; }= new List<UavRecord>();
		public List<UavPathsRecord> UavPathsRecords { get; set; } = new List<UavPathsRecord>();
		public List<UavFuelLeaksRecord> FuelLeaksRecord { get; set; }= new List<UavFuelLeaksRecord>();
		public List<Prompt> Prompts { get; set; } = new List<Prompt>();
		public List<NFZRecord> NFZRecords { get; set; } = new List<NFZRecord>();
	}
}