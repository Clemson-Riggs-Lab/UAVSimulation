using System.Collections.Generic;
using Modules.Prompts;

namespace IOHandlers
{
	public static class DefaultRecordsCreator
	{

		public static List<NFZRecord> GetDefaultNFZRecords()
		{
			var records = new List<NFZRecord>()
{
    new()
    {
        StartCoordinates = { X = 0, Y = 0, Z = 0 },
        EndCoordinates = { X = 110, Y = 300, Z = 110 },
        TextOnNFZAfterCountdown = "NFZ",
        NFZCountdownTimer = 0,
        NFZStartTime = 0,
        NFZEndTime = 0
    },
    new()
    {
        StartCoordinates = { X = 0, Y = 0, Z = 445 },
        EndCoordinates = { X = 110, Y = 300, Z = 555 },
        TextOnNFZAfterCountdown = "NFZ",
        NFZCountdownTimer = 0,
        NFZStartTime = 0,
        NFZEndTime = 0
    },
    new()
    {
        StartCoordinates = { X = 0, Y = 0, Z = 890 },
        EndCoordinates = { X = 110, Y = 300, Z = 1000 },
        TextOnNFZAfterCountdown = "NFZ",
        NFZCountdownTimer = 0,
        NFZStartTime = 0,
        NFZEndTime = 0
    },
    new()
    {
        StartCoordinates = { X = 445, Y = 0, Z = 0 },
        EndCoordinates = { X = 555, Y = 300, Z = 110 },
        TextOnNFZAfterCountdown = "NFZ",
        NFZCountdownTimer = 0,
        NFZStartTime = 0,
        NFZEndTime = 0
    },
    new()
    {
	    StartCoordinates = { X = 445, Y = 0, Z = 445 },
	    EndCoordinates = { X = 555, Y = 300, Z = 555 },
	    TextOnNFZAfterCountdown = "NFZ",
	    NFZCountdownTimer = 0,
	    NFZStartTime = 0,
	    NFZEndTime = 0
    },
    new()
    {
        StartCoordinates = { X = 445, Y = 0, Z = 890 },
        EndCoordinates = { X = 555 , Y = 300, Z = 1000 },
        TextOnNFZAfterCountdown = "NFZ",
        NFZCountdownTimer = 0,
        NFZStartTime = 0,
        NFZEndTime = 0
    },
    new()
    {
        StartCoordinates = { X = 890, Y = 0, Z = 0 },
        EndCoordinates = { X = 1000, Y = 300, Z = 110 },
        TextOnNFZAfterCountdown = "NFZ",
        NFZCountdownTimer = 0,
        NFZStartTime = 0,
        NFZEndTime = 0
    },
    new()
    {
        StartCoordinates = { X = 890, Y = 0, Z = 445 },
        EndCoordinates = { X = 1000, Y = 300, Z = 555 },
        TextOnNFZAfterCountdown = "NFZ",
        NFZCountdownTimer = 0,
        NFZStartTime = 0,
        NFZEndTime = 0
    },
    new()
    {
        StartCoordinates = { X = 890, Y = 0, Z = 890 },
        EndCoordinates = { X = 1000, Y = 300, Z = 1000 },
        TextOnNFZAfterCountdown = "NFZ",
        NFZCountdownTimer = 0,
        NFZStartTime = 0,
        NFZEndTime = 0
    }
};

			return records;
		}


		public static List<UavDynamicNavigationWorkloadRecord> GetDynamicNavigationWorkloadRecords()
		{
			var records = new List<UavDynamicNavigationWorkloadRecord>()
			{
				new () {TimeOfChange = 0, RatioOfVisibleUavsForRerouting = 0.5f,FrequencyOfHeadToNFZ = 0.1f},
				new () {TimeOfChange = 50, RatioOfVisibleUavsForRerouting = 0.8f,FrequencyOfHeadToNFZ = 0.1f},
			};
			return records;
		}

		public static List<UavDynamicTargetDetectionWorkloadRecord> GetUavDynamicTargetDetectionWorkloadRecords()
		{
			var records = new List<UavDynamicTargetDetectionWorkloadRecord>()
			{
				new () {TimeOfChange = 0, RatioOfVisibleUavsForTargetDetection = 0.5f, RatioOfFeedsWithTarget = 0.1f, RatioOfFeedsWithNonTarget = 0},
				new () {TimeOfChange = 50, RatioOfVisibleUavsForTargetDetection = 0.8f, RatioOfFeedsWithTarget = 0.1f, RatioOfFeedsWithNonTarget = 0},
			};
			return records;
		}
		
	
		public static List<UavFuelLeaksRecord> GetDefaultFuelLeaksRecord()
		{
			var records = new List<UavFuelLeaksRecord>()
			{
				new () { UavID = 0, FuelLeakTimes = new List<float>() {1f,179f} },
				new () { UavID = 2, FuelLeakTimes = new List<float>(){863} },
				new () { UavID = 3, FuelLeakTimes = new List<float>(){275f,471f} },
				new () { UavID = 4, FuelLeakTimes = new List<float>(){693f} },
				new () { UavID = 6, FuelLeakTimes = new List<float>(){432f} },
				new () { UavID = 7, FuelLeakTimes = new List<float>(){721f} },
				new () { UavID = 8, FuelLeakTimes = new List<float>(){85f,255f} },
				new () { UavID = 12, FuelLeakTimes =new List<float>() {841f} },
				new () { UavID = 13, FuelLeakTimes =new List<float>() {44f,626f} },
				new () { UavID = 14, FuelLeakTimes = new List<float>(){349f} },
			};

			return records;
		}
		
		public static List<Prompt> AddDefaultPromptRecords()
		{
			var messages = new List<Prompt>()
			{
				new()
				{
					timeToPresent = 0,
					durationToAcceptResponses = 0,
					consoleMessage = new()
					{text = "beginning Mission",
					color = "Yellow"}
				},
				new()
				{
					timeToPresent = 15,
					durationToAcceptResponses = 0,
					consoleMessage = new()
					{
						text = "Which UAV/s had a bad projected route?",
						color = "red",
					},
					acceptMultipleResponses = true,
					responseOptions = new List<ResponseOption>()
					{
						new() { buttonText = "#0"  },
						new () { buttonText = "#1" },
						new () { buttonText = "#2" },
						new () { buttonText = "#3" },
						new () { buttonText = "#4" },
						new () { buttonText = "#5" },
						new () { buttonText = "#6" },
						new () { buttonText = "#7" },
						new () { buttonText = "#8" },
						new () { buttonText = "#9" },
						new () { buttonText = "#10", isCorrectResponse = true },
						new () { buttonText = "#11" },
						new () { buttonText = "#12" },
						new () { buttonText = "#13" },
						new () { buttonText = "#14" },
						new () { buttonText = "#15" },
						
					}
				},
				new()
				{
					timeToPresent = 22,
					durationToAcceptResponses = 10,
					consoleMessage = new()
					{text = "Detecting UAV 6 flying away from NFZ?",
					color = "Yellow"},
					acceptMultipleResponses = false,
					responseOptions = new List<ResponseOption>()
					{
						new ()
						{
							buttonText = "Confirm",
							buttonColor = "Green",
							isCorrectResponse = true
						},
						new ()
						{
							buttonText = "Deny",
							buttonColor = "Red",
							isCorrectResponse = false
						},
					}
				},
				new()
				{
					timeToPresent = 129,
					durationToAcceptResponses = 0,
					consoleMessage = new()
					{text = "Is UAV 11 headed for the NFZ?",
					color = "red"},
					acceptMultipleResponses = false,
					responseOptions = new List<ResponseOption>()
					{
						new ()
						{
							buttonText = "Yes",
							isCorrectResponse = false
						},
						new ()
						{
							buttonText = "No",
							isCorrectResponse = true
						},
						new ()
						{
							buttonText = "Pass",
							isCorrectResponse = false
						}
					}
				},
				new()
				{
					timeToPresent = 161,
					durationToAcceptResponses = 0,
					consoleMessage = new()
						{
					text = "Which UAV/s had a bad projected route?",
					color = "red",
					},
					acceptMultipleResponses = true,
					responseOptions = new List<ResponseOption>()
					{
						new() { buttonText = "#0"  },
						new () { buttonText = "#1" },
						new () { buttonText = "#2" },
						new () { buttonText = "#3" },
						new () { buttonText = "#4" },
						new () { buttonText = "#5" },
						new () { buttonText = "#6" },
						new () { buttonText = "#7" },
						new () { buttonText = "#8" },
						new () { buttonText = "#9" },
						new () { buttonText = "#10", isCorrectResponse = true },
						new () { buttonText = "#11" },
						new () { buttonText = "#12" },
						new () { buttonText = "#13" },
						new () { buttonText = "#14" },
						new () { buttonText = "#15" },
						
					}
				},
				new()
				{
					timeToPresent = 204,
					durationToAcceptResponses = 0,
					consoleMessage = new()
					{
					text = "Detecting semi-transparent targets?"
					},
					acceptMultipleResponses = false,
					responseOptions = new List<ResponseOption>()
					{
						new ()
						{
							buttonText = "Confirm",
							isCorrectResponse = true
						},
						new ()
						{
							buttonText = "Deny",
							isCorrectResponse = false
						},
						new ()
						{
							buttonText = "Multiple",
							isCorrectResponse = true
						}
					}
				},
				new()
				{
					timeToPresent = 389,
					durationToAcceptResponses = 0,
					consoleMessage = new()
					{
						text = "Have all UAVs detected at least one target?",
						color = "Yellow"
					},
					acceptMultipleResponses = false,
					responseOptions = new List<ResponseOption>()
					{
						new ()
						{
							buttonText = "Yes",
							isCorrectResponse = true
						},
						new ()
						{
							buttonText = "No",
							isCorrectResponse = false
						},
					}
				},
				//414	2	Detected a FL for UAV 4  - Respond (Confirm/Deny)	Confirm
				new()
				{
					timeToPresent=414,
					durationToAcceptResponses=0,
					consoleMessage = new()
					{
					text="Detected a FL for UAV 4?"
					
					},
					acceptMultipleResponses=false,
					responseOptions=new List<ResponseOption>()
					{
						new()
						{
							buttonText="Confirm",
							isCorrectResponse=true
						},
						new()
						{
							buttonText="Deny",
							isCorrectResponse=false
						},
					}
				},
				// 492	3	Status update for the projected route for UAV 9 - Respond (Good/Bad)	Good
				new()
				{
					timeToPresent=492,
					durationToAcceptResponses=0,
					consoleMessage = new()
					{
					text="Status update for the projected route for UAV 9?"},
					acceptMultipleResponses=false,
					responseOptions=new List<ResponseOption>()
					{
						new()
						{
							buttonText="Good",
							isCorrectResponse=true
						},
						new()
						{
							buttonText="Bad",
							isCorrectResponse=false
						},
					}
				},
				// 566	1	Did UAV 3 experience a FL? - Respond (Yes/No)	No
				new()
				{
					timeToPresent=566,
					durationToAcceptResponses=0,
					consoleMessage = new() {text="Did UAV 3 experience a FL?"},
					acceptMultipleResponses=false,
					responseOptions=new List<ResponseOption>()
					{
						new()
						{
							buttonText="Yes",
							isCorrectResponse=false
						},
						new()
						{
							buttonText="No",
							isCorrectResponse=true
						},
					}
				},
				// 602	2	Detecting ground vehicles in mission- Respond (Confirm/Deny)	Deny
				new()
				{
					timeToPresent=602,
					durationToAcceptResponses=0,
					consoleMessage = new()
					{text="Detecting ground vehicles in mission?"},
					acceptMultipleResponses=false,
					responseOptions=new List<ResponseOption>()
					{
						new()
						{
							buttonText="Confirm",
							isCorrectResponse=false
						},
						new()
						{
							buttonText="Deny",
							isCorrectResponse=true
						},
					}
				},
				// 649	1	Was UAV 6 ever headed for the NFZ? - Respond (Yes/No)	Yes
				new()
				{
					timeToPresent=649,
					durationToAcceptResponses=0,
					consoleMessage = new()
					{text="Was UAV 6 ever headed for the NFZ?"},
					acceptMultipleResponses=false,
					responseOptions=new List<ResponseOption>()
					{
						new()
						{
							buttonText="Yes",
							isCorrectResponse=true
						},
						new()
						{
							buttonText="No",
							isCorrectResponse=false
						},
					}
				},
				// 672	3	Status update for the video feed of UAV 15 - Respond (Good/Bad)	Good
				new()
				{
					timeToPresent=672,
					durationToAcceptResponses=0,
					consoleMessage = new()
					{text="Status update for the video feed of UAV 15?"},
					acceptMultipleResponses=false,
					responseOptions=new List<ResponseOption>()
					{
						new()
						{
							buttonText="Good",
							isCorrectResponse=true
						},
						new()
						{
							buttonText="Bad",
							isCorrectResponse=false
						},
					}
				},
				// 819	1	Was UAV 13 ever headed for the NFZ? - Respond (Yes/No)	No
				new()
				{
					timeToPresent=819,
					durationToAcceptResponses=0,
					consoleMessage = new()
					{text="Was UAV 13 ever headed for the NFZ?"},
					acceptMultipleResponses=false,
					responseOptions=new List<ResponseOption>()
					{
						new()
						{
							buttonText="Yes",
							isCorrectResponse=false
						},
						new()
						{
							buttonText="No",
							isCorrectResponse=true
						},
					}
				},
				// 884	2	Detecting less than ten active UAVs - Respond (Confirm/Deny)	Deny
				new()
				{
					timeToPresent=884,
					durationToAcceptResponses=0,
					consoleMessage = new()
					{text="Detecting less than ten active UAVs?"},
					acceptMultipleResponses=false,
					responseOptions=new List<ResponseOption>()
					{
						new()
						{
							buttonText="Confirm",
							isCorrectResponse=false
						},
						new()
						{
							buttonText="Deny",
							isCorrectResponse=true
						},
					}
				},

			};

			return messages;
		}
	}
}