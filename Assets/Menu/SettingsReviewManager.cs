using System;
using System.Collections;
using System.Collections.Generic;
using HelperScripts;
using Menu;
using UnityEditor;
using UnityEngine;

public class SettingsReviewManager : MonoBehaviour
{
	[SerializeField] public GameObject reviewSettingsContainer;
	[SerializeField] public GameObject keyValuePairPrefab;

	private Dictionary<string, KeyValuePairController> _settingsDatabase =
		new Dictionary<string, KeyValuePairController>();

	private void OnValidate()
	{
		MyDebug.AssertComponentReferencedInEditor(reviewSettingsContainer, this,this.gameObject);
		MyDebug.AssertComponentReferencedInEditor(keyValuePairPrefab, this,this.gameObject);
	}

	void Start()
	{
		PopulateSettingsDatabase();
	}

	private void PopulateSettingsDatabase()
	{
		var kvpcScripts = GetComponentsInChildren<KeyValuePairController>();
		foreach (var kvpcScript in kvpcScripts)
		{
			var keyText = kvpcScript.GetKeyText();
			_settingsDatabase.Add(keyText, kvpcScript);
			Debug.Log(keyText);
		}
	}

	public void SetKeyValuePair(string keyText, string valueText,
		MessageType keyMessageType = MessageType.None,
		MessageType valueMessageType = MessageType.None)
	{
		KeyValuePairController kvpcScript = null;
		if (!_settingsDatabase.TryGetValue(keyText, out kvpcScript)) //if the key is not in the dictionary, create a new pair (object) and add them to the dictionary
		{
			var keyValuePairGameObject = Instantiate(keyValuePairPrefab, reviewSettingsContainer.transform);
			kvpcScript = keyValuePairGameObject.GetComponent<KeyValuePairController>();
			_settingsDatabase.Add(keyText, kvpcScript);
		}

		//setting the key and the value of the new or previously existing key-value pair
		kvpcScript.SetKeyText(keyText, keyMessageType);
		kvpcScript.SetValueText(valueText, valueMessageType);
	}
}