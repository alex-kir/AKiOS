using System;

namespace AKiOS
{
	public class UIWindow : UIView
	{
		public UIViewController RootViewController {
			get { return this.Call("rootViewController").Cast<UIViewController>(); }
		}
		
		public UIWindow ()
		{
		}
	}
}

