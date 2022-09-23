using ScriptableObjects.EventChannels;
using UI.Console.Channels.ScriptableObjects;
using UnityEngine;

namespace HelperScripts
{
	public class ConsoleDebug: MonoBehaviour
	{
		[SerializeField] private static ConsoleMessageEventChannelSO _writeMessageToConsoleChannel;

		private static void RaiseError(string errorText)
		{
			if(_writeMessageToConsoleChannel != null)
				_writeMessageToConsoleChannel.RaiseEvent("",new() {text= errorText,color = "red"});
		}
	}
	
}