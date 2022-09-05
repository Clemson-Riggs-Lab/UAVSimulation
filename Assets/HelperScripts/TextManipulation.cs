using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace HelperScripts
{
    public static class TextManipulation
    {
        public static string AddColorToText(string text, string color)
        {
            var textColor = ColorHelper.StringToColor(color);
            var textWithColor = "<color=#" + textColor.ToHexString() + ">" + text + "</color>";
            return textWithColor;
        }
    }
}