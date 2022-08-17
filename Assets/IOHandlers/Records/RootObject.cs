using System;
using System.Collections.Generic;
using Chat;

namespace IOHandlers.Records
{
	public class RootObject
	{
		public SettingsRecord Settings { get; set; } = new SettingsRecord();
		public List<WayPointRecord> WayPointsRecords { get; set; } = new List<WayPointRecord>();
		public List<UavRecord> UavsRecords { get; set; }= new List<UavRecord>();
		public List<UavPathsRecord> UavPathsRecords { get; set; } = new List<UavPathsRecord>();
		public List<UavFuelLeaksRecord> FuelLeaksRecord { get; set; }= new List<UavFuelLeaksRecord>();
		public List<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
	}

	
}