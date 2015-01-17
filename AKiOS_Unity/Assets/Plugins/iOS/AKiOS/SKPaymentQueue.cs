using System;

namespace AKiOS
{
	public class SKPaymentQueue : NSObject
	{
		public static SKPaymentQueue DefaultQueue
		{
			get
			{
				return Class.FindByName("SKPaymentQueue").Call("defaultQueue").Cast<SKPaymentQueue>();
			}
		}
		
		public void AddTransactionObserver(NSObject queueObserver)
		{
			this.Call("addTransactionObserver:", queueObserver);
		}
		
		public void AddPayment(SKPayment payment)
		{
			this.Call("addPayment:", payment);
		}
		
		public void FinishTransaction(SKPaymentTransaction transaction)
		{
			this.Call("finishTransaction:", transaction);
		}
		
		public void RestoreCompletedTransactions()
		{
			this.Call("restoreCompletedTransactions");
		}
		
	}
}

