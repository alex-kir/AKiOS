using System;

namespace AKiOS
{
	public class SKProductsRequest : NSObject
	{
		public static SKProductsRequest AllocInitWithProductIdentifiers(NSMutableSet productIds)
		{
			return Class.FindByName("SKProductsRequest").Call("alloc").Call("initWithProductIdentifiers:", productIds).Cast<SKProductsRequest>();
		}
		
		public void SetDelegate(NSObject productDelegate)
		{
			this.Call("setDelegate:", productDelegate);
		}
		
		public void Start()
		{
			this.Call("start");
		}
	}
}

