using System;

namespace AKiOS
{
	public class UIScreen : NSObject
	{
		public static UIScreen MainScreen { get { return Class.FindByName("UIScreen").Call("mainScreen").Cast<UIScreen>(); } }
		public float Scale { get { return this.Call ("scale").AsFloat(); } }
		
		public UIScreen ()
		{
		}
	}
}

