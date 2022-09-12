using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UAVs.Sub_Modules.Rerouting.Panel
{
	public class ReroutingOptionRowController:MonoBehaviour
	{
		public TextMeshProUGUI routeLabel;
		public Button previewButton;
		public Button cancelButton;
		public Button confirmButton;
		public Image backgroundImage;

		public void Highlight(bool option)
		{
			var color = backgroundImage.color;
			if(option)
			{
				color.a = 1;
				backgroundImage.color = color;
			}
			else
			{
				color.a = 0f;
				backgroundImage.color = color;
			}
		}
		
	}
}
