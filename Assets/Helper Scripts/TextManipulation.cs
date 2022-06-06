using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Helper_Scripts
{
    public static class TextManipulation
    {
        public static string AddColorToText(string text, MessageType messageType)
        {
            var textColor = GetColorBasedOnMessageType(messageType);
            var textWithColor = "<color=#" + textColor.ToHexString() + ">" + text + "</color>";
            return textWithColor;
        }
        
        
        private static Color GetColorBasedOnMessageType(MessageType messageType)
        {
            return messageType switch
            {
                MessageType.None => Color.black,
                MessageType.Info => Color.cyan,
                MessageType.Warning => Color.yellow,
                MessageType.Error => Color.red,
                _ => Color.black
            };
        }
    }
}