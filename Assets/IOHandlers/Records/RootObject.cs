using System;
using System.Collections.Generic;

namespace IOHandlers.Records
{
	public class RootObject
	{
		public SettingsRecord Settings { get; set; } = new SettingsRecord();
		public List<WaypointRecord> WaypointsRecords { get; set; } = new List<WaypointRecord>();
		public List<UavRecord> UavsRecords { get; set; }= new List<UavRecord>();
		public List<UavPathsRecord> UavPathsRecords { get; set; } = new List<UavPathsRecord>();
		public List<UavFuelLeaksRecord> FuelLeaksRecord { get; set; }= new List<UavFuelLeaksRecord>();
	}

	
}