Unity3D Configs from Google Spreadsheet
============
Use Google Docs Spreadsheet to store your app or game configurations that you can edit on the fly. This is especially useful when testing your app on mobile to save you time from compilation.

Then once you are done testing, use it to save your config in a JSON text format.


Overview
--------
Based on a [blog][1] post by Robert Yang, you can read public Google Spreadsheet documents using the .Net version of [Google Data API][2].

Unfortunately, the .Net API does not work when built for mobile. Or more accurately, one of its dependencies, [Json.Net][3] have [issues][4] when compiled for iOS. 

So rather than fight more battles in making the .Net library work for mobile, I decided to use the [Spreadsheet REST API][5] instead which outputs JSON data.

Setup
-----
You will need to create a spreadsheet with rows defining your key-value pair data.

![Sample Spreadseet Format][8]

To make the spreadsheet readable, you have to make it 'public'. Here's a quote from Robert's blog:
```
Open up your spreadsheet in Google Docs and go to 
File >> Publish to the Web, and publish it to the web. 

To make it publicly accessible to the API, you MUST publish it publicly like this! 
You CANNOT just click the "Share" button and change access rights to share with anyone, 
that's something different according to the API. Yes, it's confusing.
```

Once you have published the spreadsheet, take note of your document key from the URL. It's usually in the format 
```
https://docs.google.com/spreadsheets/d/{DOCUMENT_KEY_HERE}
```

Usage
-----
It uses generics so you can define your configuration class as below:
```csharp
public class GameConfig {
	public string text;
	public int integerValue;
	public int[] integerArray;
	public float decimalValue;
	public float[] decimalArray;
}
```

Your class' property names must correspond to the key names in your spreadsheet. Then you can read your spreadsheet in just one line:
```csharp
GameConfig config = GDocConfig.LoadUsingKey<GameConfig>("ENTER GDOC KEY HERE");
Debug.Log(config.text);
```

You can then save it to Unity's Resources folder for offline access later. 
Preferably, you add this to your build process so you can cache the latest Spreadsheet data offline. 
You will also want to do this when releasing your game so you don't accidentally edit a live config while players are using it.
```
GDocConfig.SaveToResources<GameConfig>(null, "ENTER GDOC KEY HERE");
```

And open it later from Resources folder simply by doing:
```
GameConfig config = GDocConfig.LoadFromResources<GameConfig>();
```


Dependencies
------------
[JsonFX][6]
[LitJSON][7]

[1]: http://www.blog.radiator.debacle.us/2013/12/reading-public-google-drive.html
[2]: https://code.google.com/p/google-gdata/downloads/list
[3]: https://github.com/JamesNK/Newtonsoft.Json
[4]: https://github.com/JamesNK/Newtonsoft.Json/issues/219
[5]: https://developers.google.com/gdata/samples/spreadsheet_sample
[6]: http://www.jsonfx.net/
[7]: http://lbv.github.io/litjson/
[8]: http://s22.postimg.org/i1hda6jxd/Sample_Sheet_Format.png
