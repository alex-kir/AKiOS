using System;

namespace AKiOS
{
	public class SKPaymentTransaction : NSObject
	{
		public const int SKPaymentTransactionStatePurchasing = 0;
		public const int SKPaymentTransactionStatePurchased = 1;
		public const int SKPaymentTransactionStateFailed = 2;
		public const int SKPaymentTransactionStateRestored = 3;
		
		
		public const int SKErrorUnknown = 0;
		public const int SKErrorClientInvalid = 1;
		public const int SKErrorPaymentCancelled = 2;
		public const int SKErrorPaymentInvalid = 3;
		public const int SKErrorPaymentNotAllowed = 4;
		public const int SKErrorStoreProductNotAvailable = 5;

		
		public int TransactionState { get { return this.Call("transactionState").AsInt32(); } }
		public SKPayment Payment { get { return this.Call("payment").Cast<SKPayment>(); } }
		
		public NSError Error { get { return this.Call("error").Cast<NSError>(); } }
		
	}
}

