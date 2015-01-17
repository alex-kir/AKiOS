using System;

namespace AKiOS
{
	public class SKPayment : NSObject
	{
		public NSString ProductIdentifier { get { return this.Call("productIdentifier").Cast<NSString>(); } }

		public static SKPayment PaymentWithProduct(SKProduct product)
		{
			return Class.FindByName("SKPayment").Call("paymentWithProduct:", product).Cast<SKPayment>();
		}

	}
}

