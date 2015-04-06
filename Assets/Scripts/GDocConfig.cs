//#define DEBUG_VALUES
using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Reflection;

public static class GDocConfig {

	public static T LoadUsingKey<T>(string key) where T : new() {
		SpreadsheetListFeed listFeed = GDocService.GetSpreadsheetContents(key);
		if(listFeed == null) {
			Debug.LogWarning("GameConfig spreadsheet was not loaded. Loading gameconfig.txt instead.");
			return LoadFromResources<T>();
		}

		Hashtable tableRows = listFeed.GetRows();
		T config = new T();
		PropertyInfo[] properties = config.GetType().GetProperties();
		for(int i=0; i<properties.Length; ++i) {
			PropertyInfo property = properties[i];
			if(tableRows.ContainsKey(property.Name)) {
				Hashtable tableColumns = tableRows[property.Name] as Hashtable;
				if(tableColumns.Contains("value")){
					Log("Value of " + property.Name + " : " + tableColumns["value"]);
					Log("Before: " + property.GetValue(config, null));
					property.SetValue(config, Convert.ChangeType(tableColumns["value"], property.PropertyType), null);
					Log("After: " + property.GetValue(config, null));
				}
			}
		}
		return config;
	}

	public static void SaveToResources<T>(T data, string key) where T : new() {
		if(!Application.isEditor) {
			Debug.LogWarning("GameConfig can only be saved from the Editor");
			return;
		}
		
		if(data == null) {
			data = LoadUsingKey<T>(key);
			if(data == null) {
				Debug.LogWarning("Could not load GameConfig from GDoc");
				return;
			}
		}
		
		string json = JsonFx.Json.JsonWriter.Serialize(data);
		string jsonPath = Path.Combine(Path.Combine(GetProjectPath(), "Day1/Resources"), "gameconfig.txt");
		File.WriteAllText(jsonPath, json);
		Log("GameConfig:\n " + json);
	}

	public static T LoadFromResources<T>() {
		string config = "";
		TextAsset configAsset = Resources.Load<TextAsset>("gameconfig");
		if(configAsset != null)
			config = configAsset.text;
		else
			Debug.LogWarning("Could not load local gameconfig file.");
		
		T data = default(T);
		try {
			data = JsonFx.Json.JsonReader.Deserialize<T>(config);
		} catch(Exception e) {
			Debug.LogWarning("Could not read gameconfig file. " + e.Message);
		}
		
		return data;
	}

	static string GetProjectPath() {
		string codeBase = Assembly.GetExecutingAssembly().CodeBase;
		UriBuilder uri = new UriBuilder(codeBase);
		string path = Uri.UnescapeDataString(uri.Path);
		return Path.GetFullPath(new Uri(Path.Combine(Path.GetDirectoryName(path), "../../Assets")).AbsolutePath);
	}

	static void Log(string msg) {
		#if DEBUG_VALUES
		Debug.Log(msg);
		#endif
	}
}
