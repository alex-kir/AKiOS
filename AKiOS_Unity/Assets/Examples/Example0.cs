using UnityEngine;
using System.Collections;
using System;

public class Example0
{
	public string Name = "?";
	
	public static Action<string> LOG = t => { };
		
	public Example0()
	{
		Name = this.GetType().Name.Replace("Example_", "").Replace("_", " ");
	}
	
	public void Run()
	{
		LOG(Name + " started");
		try
		{
			this.OnRun();
			LOG(Name + " complete");
		}
		catch(Exception ex)
		{
			LOG(Name + " failed " + ex.GetType().Name + " " + ex.Message);
		}
	}

	protected virtual void OnRun()
	{
	}

}
