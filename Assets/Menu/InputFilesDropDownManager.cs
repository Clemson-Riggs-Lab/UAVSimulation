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
			MyDebug.AssertComponentReferencedInEditor(dropdown, this.gameObject);
			MyDebug.AssertComponentReferencedInEditor(inputFilesHandler, this.gameObject);
			MyDebug.AssertComponentReferencedInEditor(consoleTextHandler, this.gameObject);
		}

		void Start()
		{
			PopulateDatabaseAndDropdown(inputFilesHandler.GetFilesInfo(inputFilesType), initialized: false);
			dropdown.onValueChanged.AddListener(DropDownValueChanged);
		}

		public void PopulateDatabaseAndDropdown(List<FileInfo> filesInfo, bool initialized = true)
		{
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
							$"Trying to add File ({fileInfo.FullName}) which is already in the dropdown list. The file is now selected as the input file.",
							MessageType.Warning, false);

						dropdown.value = _filesDatabase.Values.ToList().IndexOf(fileInfo);
					}
				}
			}

			dropdown.ClearOptions(); //removing all options (old)
			dropdown.AddOptions(_filesDatabase.Keys.ToList()); //adding all options (old + new)

			if (!initialized) RefreshDropDown();
			else
				RefreshDropDown(selectedIndex: dropdown.options.Count - 1); //set selected to last option (most recently added)
		}


		private void RefreshDropDown(int selectedIndex = 0)
		{
			dropdown.value = selectedIndex;
			dropdown.RefreshShownValue();
		}

		private void DropDownValueChanged(int position)
		{
			inputFilesHandler.SelectedBaseInputFileInfo = _filesDatabase.Values.ElementAt(position);
		}
	}
}