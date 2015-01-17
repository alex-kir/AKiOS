using System;

namespace AKiOS
{
	public class GADBannerView : UIView
	{
		public UIViewController RootViewController { set { this.Call("setRootViewController:", value); } }
		public NSString AdUnitID { set { this.Call("setAdUnitID:", value); } }
		public NSObject Delegate { set { this.Call("setDelegate:", value); } }
		
		public static GADBannerView AllocInitWithAdSize(GADAdSize size)
		{
			return Class.FindByName("GADBannerView")
				.Call("alloc")
				.Call("initWithAdSize:", size).Cast<GADBannerView>();
		}

		public void LoadRequest(GADRequest request)
		{
			this.Call("loadRequest:", request);
		}
	}
}

