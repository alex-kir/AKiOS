using System;

namespace AKiOS
{
	public class GADInterstitial : NSObject
	{
		public NSString AdUnitID { set { this.Call("setAdUnitID:", value); } }
		public NSObject Delegate { set { this.Call("setDelegate:", value); } }

		public static GADInterstitial New()
		{
			return Class.FindByName("GADInterstitial").Call("new").Cast<GADInterstitial>();
		}

		public void LoadRequest(GADRequest request)
		{
			this.Call("loadRequest:", request);
		}

	}
}

