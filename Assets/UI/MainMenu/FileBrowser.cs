using System;
using System.IO;
using System.Linq;
using HelperScripts;
using IOHandlers;
using SFB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
	public class FileBrowser : MonoBehaviour
	{
		[SerializeField] public Button button;
		[SerializeField] public InputFilesDropDownManager dropDownManager;
		[SerializeField] public InputFilesHandler inputFilesHandler;

		private void OnValidate()
		{
			AssertionHelper.AssertComponentReferencedInEditor(button, this,this.gameObject);
			AssertionHelper.AssertComponentReferencedInEditor(dropDownManager, this,this.gameObject);
			AssertionHelper.AssertComponentReferencedInEditor(inputFilesHandler, this,this.gameObject);
		}

		void Start()
		{
			if (Application.platform is not (RuntimePlatform.WindowsEditor or RuntimePlatform.WindowsPlayer))
			{
				button.enabled = false;
				this.transform.parent.gameObject.SetActive(false);
			}
			else
				button.onClick.AddListener(ShowBrowseFileDialog);
		}

		private void ShowBrowseFileDialog()
		{ 
			//open file dialog without blocking the main thread
			
			var paths =  StandaloneFileBrowser.OpenFilePanel("Open File",
				Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "json", true);
			var filesInfo = paths.Select(path => new FileInfo(path)).ToList();
			if (paths.Length == 1) //only one file selected, then the user probably wants this input file, and there is no need to do the extra step of selecting it from the Dropdown menu.
				inputFilesHandler.SelectedBaseInputFileInfo = filesInfo[0];

			dropDownManager.PopulateDatabaseAndDropdown(filesInfo);
		}
	}
}