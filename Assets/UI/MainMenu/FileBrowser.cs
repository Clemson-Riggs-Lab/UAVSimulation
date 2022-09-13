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
		[SerializeField] public ConfigFilesDropDownManager dropDownManager;
		[SerializeField] public ConfigFilesHandler configFilesHandler;
		[SerializeField] public string fileExtension;
		[SerializeField] public ConfigFilesHandler.FilesType fileType;
		
		private void OnValidate()
		{
			AssertionHelper.AssertComponentReferencedInEditor(button, this,this.gameObject);
			AssertionHelper.AssertComponentReferencedInEditor(dropDownManager, this,this.gameObject);
			AssertionHelper.AssertComponentReferencedInEditor(configFilesHandler, this,this.gameObject);
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
			var paths = StandaloneFileBrowser.OpenFilePanel("Select " + fileType.ToString() + " file",
				Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileExtension, false);
			
			var filesInfo = paths.Select(path => new FileInfo(path)).ToList();

			if (filesInfo.Count > 0 && filesInfo[0].FullName.Contains("." + fileType.ToString() + "." + fileExtension))  //check if the file is of the correct type
			{
				dropDownManager.AddItemsToDatabaseAndDropdown(filesInfo);
				configFilesHandler.SelectFile(filesInfo[0], fileType);
			}
			else
			{
				configFilesHandler.WriteToConsole("File type is not " + fileType.ToString() + "!","red");
			}

		}
	}
}