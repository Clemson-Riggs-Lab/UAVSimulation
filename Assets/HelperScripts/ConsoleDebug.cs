using ScriptableObjects.EventChannels;
using UnityEngine;

namespace HelperScripts
{
	public class ConsoleDebug: MonoBehaviour
	{
		[SerializeField] private static ConsoleMessageEventChannelSO writeMessageToConsoleChannel;

		private static void RaiseError(string errorText)
		{
			if(writeMessageToConsoleChannel != null)
				writeMessageToConsoleChannel.RaiseEvent("",new() {text= errorText,color = "red"});
		}
	}
	
}