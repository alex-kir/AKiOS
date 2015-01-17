using System;

namespace AKiOS
{
	public class SKProductsResponse : NSObject
	{
		public NSArray<SKProduct> Products { get { return this.Call("products").Cast<NSArray<SKProduct>>(); } }
		public NSArray<NSString> InvalidProductIdentifiers { get { return this.Call("invalidProductIdentifiers").Cast<NSArray<NSString>>(); } }

		public SKProductsResponse()
		{
		}
	}
}

