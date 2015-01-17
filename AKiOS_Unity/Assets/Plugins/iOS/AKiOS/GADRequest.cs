using System;

namespace AKiOS
{
	public class GADRequest : NSObject
	{
		public NSArray<NSString> TestDevices { set { this.Call("setTestDevices:", value); } }
		
		public static GADRequest New()
		{
			return Class.FindByName("GADRequest").Call("new").Cast<GADRequest>();
		}
		
		public GADRequest ()
		{
		}
	}
}

