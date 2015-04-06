// To prepare a Google Spreadsheet for this:
// make sure the spreadsheet has been published to web (FILE >> PUBLISH TO WEB), which is NOT the same as sharing to web

using UnityEngine;
using System.Collections;
using System.IO;
using LitJson;

// for "InsecureSecurityPolicy" class at the end
using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class SpreadsheetListFeed {

	public string version { get; set;}
	public string encoding { get; set;}
	public JsonData feed { get; set;}

	public Hashtable GetRows() {
		Hashtable rows = new Hashtable();
		var entries = feed["entry"];
		for(int entryIndex=0; entryIndex<entries.Count; ++entryIndex) {
			var entry = entries[entryIndex];
			string title = (string)entry["title"]["$t"];
			string content = (string)entry["content"]["$t"];

			// Parse content from format:
			// "value: 30, description: Player max health"
			string[] data = content.Split(new string[]{", "}, StringSplitOptions.None);
			if(data.Length < 1) {
				Debug.LogWarning(string.Format("Unable to parse row: {0} | {1}", title, content));
				continue;
			}

			Hashtable valueTable = new Hashtable();
			for(int dataIndex=0; dataIndex<data.Length; ++dataIndex) {
				string[] values = data[dataIndex].Split(':');
				if(values.Length < 2) {
					Debug.LogWarning(string.Format("Unable to parse value: {0} | {1}", title, content));
					continue;
				}
				valueTable.Add(values[0].Trim(), values[1].Trim());
			}

			rows.Add(title, valueTable);
		}
		return rows;
	}

}

public class GDocService {

	public static SpreadsheetListFeed GetSpreadsheetContents(string spreadsheetID) {
		return GetSpreadsheetContents(spreadsheetID, "1");
	}

	public static SpreadsheetListFeed GetSpreadsheetContents(string spreadsheetID, string sheetID) {
		InsecureSecurityCertificatePolicy.Instate();

		SpreadsheetListFeed listFeed = null;
		string json = "";
		string url = "https://spreadsheets.google.com/feeds/list/" + spreadsheetID + "/" + sheetID + "/public/basic?alt=json";
		HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

		try {
			using(HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
				using(StreamReader reader = new StreamReader(response.GetResponseStream())) {
					json = reader.ReadToEnd();
				}
			}
//			Debug.Log(string.Format("Response from {0}\n{1}", url, json));
			listFeed = JsonMapper.ToObject<SpreadsheetListFeed>(json);
		}catch(Exception e) {
			Debug.LogWarning("Failed to read spreadsheet: " + spreadsheetID + " " + e.Message);
			listFeed = null;
		}

		return listFeed;
	}

}

// from http://answers.unity3d.com/questions/249052/accessing-google-apis-in-unity.html
public class InsecureSecurityCertificatePolicy {
	public static bool Validator(
		object sender,
		X509Certificate certificate,
		X509Chain chain,
		SslPolicyErrors policyErrors) {
		// Just accept and move on...
		return true;
	}
	
	public static void Instate() {
		ServicePointManager.ServerCertificateValidationCallback = Validator;
	}
}