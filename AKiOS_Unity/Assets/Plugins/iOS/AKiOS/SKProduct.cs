using System;

namespace AKiOS
{
	public class SKProduct : NSObject
	{
		public NSString ProductIdentifier { get { return this.Call("productIdentifier").Cast<NSString>(); } }
		public NSString LocalizedTitle { get { return this.Call("localizedTitle").Cast<NSString>(); } }
		public NSString LocalizedDescription { get { return this.Call("localizedDescription").Cast<NSString>(); } }
		
		public NSNumber Price { get { return this.Call("price").Cast<NSNumber>(); } }
		public NSLocale PriceLocale { get { return this.Call("priceLocale").Cast<NSLocale>(); } }
	}
}

