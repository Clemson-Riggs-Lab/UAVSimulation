using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Helper_Scripts;
using IOHandlers;
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
		[SerializeField] public ConsoleTextHandler consoleTextHandler;

		private void OnValidate()
		{
			MyDebug.AssertComponentReferencedInEditor(dropdown,this, this.gameObject);
			MyDebug.AssertComponentReferencedInEditor(inputFilesHandler, this,this.gameObject);
			MyDebug.AssertComponentReferencedInEditor(consoleTextHandler, this,this.gameObject);
		}

		void Start()
		{
			PopulateDatabaseAndDropdown(inputFilesHandler.GetFilesInfo(inputFilesType), initialized: false);
			dropdown.onValueChanged.AddListener(DropDownValueChanged);
		}

		public void PopulateDatabaseAndDropdown(List<FileInfo> filesInfo, bool initialized = true)
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
						consoleTextHandler.AddTextToConsole(
							$"The file  ({fileInfo.Name}) is already in the dropdown list. The file is now selected as the input file.",
							MessageType.Warning, true);

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