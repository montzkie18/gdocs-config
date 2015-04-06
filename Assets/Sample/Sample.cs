using UnityEngine;
using System.Collections;

public class GameConfig {
	public string text;
	public int integerValue;
	public int[] integerArray;
	public float decimalValue;
	public float[] decimalArray;
}

public class Sample : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameConfig config = GDocConfig.LoadUsingKey<GameConfig>("ENTER GDOC KEY HERE");
		Debug.Log(config.text);

		Debug.Log(string.Format("Integer Value: {0}", config.integerValue));

		Debug.Log("IntegerArray:");
		for(int i=0; i<config.integerArray.Length; ++i)
			Debug.Log(config.integerArray[i]);

		Debug.Log(string.Format("Decimal Value: {0}", config.decimalValue));

		Debug.Log("DecimalArray:");
		for(int i=0; i<config.decimalArray.Length; ++i)
			Debug.Log(config.decimalArray[i]);
	}
}
