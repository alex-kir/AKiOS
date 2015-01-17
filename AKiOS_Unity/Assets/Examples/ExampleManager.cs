using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExampleManager : MonoBehaviour {
	
	#region Log
	
	public GUIText log = null;

	private static GUIText _log = null;
	private static void InitLog(GUIText log)
	{
		_log = log;
		if (_log)
			_log.text = "";
	}
	
	protected static void Log(string text)
	{
		Debug.Log(text);
		if (_log)
			_log.text += text + "\n";
	}
	
	protected static void ClearLog()
	{
		if (_log)
			_log.text = "";
	}
	
	#endregion
	
	private List<Example0> tests = null;
	private int index = 0;
	
	// Use this for initialization
	void Start ()
	{
		InitLog(log);
		Example0.LOG = Log;
		
		tests = new List<Example0>{
			new Example_AdMob_Banner(),
			new Example_AppStore(),
		};
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	Rect GetRect(int index)
	{
		return new Rect(Screen.width / 4 * index + 10, Screen.height * 0.85f, Screen.width / 4 - 20, Screen.height * 0.15f - 10);
	}
	
	void OnGUI()
	{
	    if (GUI.Button(GetRect(0), "RUN EXAMPLE\n" + tests[index].Name))
		{
			tests[index].Run();
		}

        if (GUI.Button(GetRect(1), "<<"))
		{
			index = (index + tests.Count - 1) % tests.Count;
		}

	    if (GUI.Button(GetRect(2), ">>"))
		{
			index = (index + 1) % tests.Count;
		}

	    if (GUI.Button(GetRect(3), "CLEAR LOG"))
		{
			ClearLog();
		}
	}
}
