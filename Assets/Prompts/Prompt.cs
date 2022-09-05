using System.Collections.Generic;

namespace Prompts
{
	public class Prompt
	{
		public float timeToPresent;
		public float durationToAcceptResponses = 0;
		public ConsoleMessage consoleMessage;
		public bool acceptMultipleResponses;
		public List<ResponseOption> responseOptions;
		

	}

	[System.Serializable]
	public class ResponseOption
	{
		public string buttonText;
		public string textColor="black";
		public string buttonColor = "Gray";
		public bool isCorrectResponse=false;
	}
	
	[System.Serializable]
	public class ConsoleMessage
	{
		public string text;
		public string color="black";
		public bool doAnimate=true;

	}
}