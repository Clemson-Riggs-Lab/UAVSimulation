using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HelperScripts;
using IOHandlers;
using ScriptableObjects.EventChannels;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Menu
{
	public class InputFilesDropDownManager : MonoBehaviour
	{
		public InputFilesHandler.FilesType inputFilesType;
		private Dictionary<string, FileInfo> _filesDatabase = new Dictionary<string, FileInfo>(); //file name, file Info

		[SerializeField] public TMP_Dropdown dropdown;
		[SerializeField] public InputFilesHandler inputFilesHandler;

		[SerializeField] private ConsoleMessageEventChannelSO writeMessageToConsoleChannel;

		private void OnValidate()
		{
			AssertionHelper.AssertComponentReferencedInEditor(dropdown,this, this.gameObject);
			AssertionHelper.AssertComponentReferencedInEditor(inputFilesHandler, this,this.gameObject);
		}

		void Start()
		{
			PopulateDatabaseAndDropdown(inputFilesHandler.GetFilesInfoFromWorkDir(inputFilesType));
			dropdown.onValueChanged.AddListener(DropDownValueChanged);
		}

		public void PopulateDatabaseAndDropdown(List<FileInfo> filesInfo)
		{
			var selectedOptionDropdownIndex = int.MinValue;

			foreach (var fileInfo in filesInfo)
			{
				if (_filesDatabase.TryAdd(fileInfo.Name, fileInfo)) continue; //added successfully
				else //if couldn't add entry (file name already exists as key in the dictionary)
				{
					var search = _filesDatabase.Where(i =>
						((i.Value.Name == fileInfo.Name) && (i.Value.FullName == fileInfo.FullName)));
					if (!search.Any()) // no hits on search => we have similar names but with different paths
					{
						_filesDatabase.Add(fileInfo.FullName,
							fileInfo); //add full path as the name for the current item
					}
					else //same path same name, so it is the same file.
					{
						if(writeMessageToConsoleChannel != null)
							writeMessageToConsoleChannel.RaiseEvent("",
								new()
								{ text = $"File {fileInfo.Name} is already in the dropdown list. The file is now selected as the input file." });

						selectedOptionDropdownIndex = _filesDatabase.Values.ToList().IndexOf(fileInfo);
					}
				}
			}
			
			RefreshDropDown(selectedIndex: selectedOptionDropdownIndex); //set selected to last option (most recently added)
		}


		private void RefreshDropDown(int selectedIndex = int.MinValue)
		{
			dropdown.ClearOptions(); //removing all options (old)
			dropdown.AddOptions(_filesDatabase.Keys.ToList()); //adding all options (old + new)
			
			dropdown.RefreshShownValue();
			if(selectedIndex!=int.MinValue)
				dropdown.value = selectedIndex;
		}

		private void DropDownValueChanged(int position)
		{
			inputFilesHandler.SelectedBaseInputFileInfo = _filesDatabase.Values.ElementAt(position);
		}
	}
}