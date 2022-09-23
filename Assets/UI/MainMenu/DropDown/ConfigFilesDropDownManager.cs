using System.Collections.Generic;
using System.IO;
using System.Linq;
using HelperScripts;
using IOHandlers;
using TMPro;
using UnityEngine;

namespace UI.MainMenu.DropDown
{
	public class ConfigFilesDropDownManager : MonoBehaviour
	{
		public ConfigFilesHandler.FilesType fileType;
		private OrderedDictionary<string, FileInfo> _filesDatabase = new OrderedDictionary<string, FileInfo>(); //file name, file Info

		[SerializeField] public TMP_Dropdown dropdown;
		[SerializeField] public ConfigFilesHandler configFilesHandler;
		
		private void OnValidate()
		{
			AssertionHelper.AssertComponentReferencedInEditor(dropdown,this, this.gameObject);
			AssertionHelper.AssertComponentReferencedInEditor(configFilesHandler, this,this.gameObject);
		}

		void Start()
		{
			AddItemsToDatabaseAndDropdown(configFilesHandler.GetFilesInfoFromWorkDir(fileType));
			dropdown.onValueChanged.AddListener(DropDownValueChanged);
		}

		public void AddItemsToDatabaseAndDropdown(List<FileInfo> filesInfo)
		{
			if (filesInfo == null || filesInfo.Count == 0) return;
			
			var selectedOptionDropdownIndex = int.MinValue;

			foreach (var fileInfo in filesInfo)
			{
				if (_filesDatabase.TryAdd(fileInfo.Name, fileInfo)) 
					selectedOptionDropdownIndex = _filesDatabase.Values.ToList().IndexOf(fileInfo); //added successfully
					
				else //if couldn't add entry (file name already exists as key in the dictionary)
				{
					var search = _filesDatabase.Where(i =>
						((i.Value.Name == fileInfo.Name) && (i.Value.FullName == fileInfo.FullName)));
					if (!search.Any()) // no hits on search => we have similar names but with different paths
					{
						_filesDatabase.Add(fileInfo.FullName, fileInfo); //add full path as the name for the current item
					}
					else //same path same name, so it is the same file.
					{
						configFilesHandler.WriteToConsole($"File {fileInfo.Name} is already in the dropdown list. It is now selected as the input file.", "yellow");
					
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
			DropDownValueChanged(dropdown.value);
		}

		private void DropDownValueChanged(int position)
		{
			configFilesHandler.SelectFile(_filesDatabase.Values.ElementAt(position), fileType);
		}
		
	}
}