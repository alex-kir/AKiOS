using System;

namespace AKiOS
{
	public class UIApplication : NSObject
	{
		public static UIApplication SharedApplication { get { return Class.FindByName("UIApplication").Call("sharedApplication").Cast<UIApplication>(); } }
		public UIWindow KeyWindow { get { return this.Call("keyWindow").Cast<UIWindow>(); } }
		
		public UIApplication ()
		{
		}
	}
}

