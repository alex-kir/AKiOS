using UnityEngine;
using System.Collections;
using AKiOS;

public class Example_AppStore : Example0
{
	static ExampleProductRequestDelegate productRequestDelegate = null;
	static ExamplePaymentQueueObserver queueObserver = null;

	class ExampleProductRequestDelegate : AKiOS.Core.ProxyObject
	{
		public ExampleProductRequestDelegate()
		{
			InitProxyObject("new");
		}
		
		[AKiOS.Core.CocoaMethod("productsRequest:didReceiveResponse:")]
		public void productsRequest_didReceiveResponse_(NSObject request, SKProductsResponse response)
		{
			AKiOS.Functions.NSLog("productsRequest:didReceiveResponse: ");
			
			foreach (var product in response.Products)
			{
				LOG("product:" + product.ProductIdentifier);
				var payment = SKPayment.PaymentWithProduct(product);
				SKPaymentQueue.DefaultQueue.AddPayment(payment);
			}
			
			foreach (var invalidProductId in response.InvalidProductIdentifiers)
			{
				LOG("invalid product id: " + invalidProductId);
			}
		}
		
		[AKiOS.Core.CocoaMethod("requestDidFinish:")]
		public void requestDidFinish_(AKiOS.Core.Arguments args)
		{
			Debug.Log("requestDidFinish: ");
		}
		
		[AKiOS.Core.CocoaMethod("request:didFailWithError:")]
		private void request_didFailWithError_(NSObject request, NSError error)
		{
			LOG("request_didFailWithError_ " + error.LocalizedDescription);
		}
	}
	
	class ExamplePaymentQueueObserver : AKiOS.Core.ProxyObject
	{
		public ExamplePaymentQueueObserver()
		{
			InitProxyObject("new");
		}
		
		[AKiOS.Core.CocoaMethod("paymentQueue:updatedTransactions:")]
		public void paymentQueue_updatedTransactions_(SKPaymentQueue queue, NSArray<SKPaymentTransaction> transactions)
		{
			LOG("paymentQueue:updatedTransactions: ");
			
			foreach (var transaction in transactions)
			{
				var productId = transaction.Payment.ProductIdentifier.ToString();
				var state = transaction.TransactionState;
				if (state == SKPaymentTransaction.SKPaymentTransactionStatePurchased)
				{
					LOG("SKPaymentTransactionStatePurchased, " + productId);
					queue.FinishTransaction(transaction);
				}
				else if (state == SKPaymentTransaction.SKPaymentTransactionStateRestored)
				{
					LOG("SKPaymentTransactionStateRestored, " + productId);
					queue.FinishTransaction(transaction);
				}
				else if (state == SKPaymentTransaction.SKPaymentTransactionStateFailed)
				{
					LOG("SKPaymentTransactionStateFailed, " + productId + ", " + transaction.Error.LocalizedDescription);
					queue.FinishTransaction(transaction);
				}
			}
		}
	}
	
	
	protected override void OnRun()
	{
		if (queueObserver == null)
		{
			queueObserver = new ExamplePaymentQueueObserver();
			SKPaymentQueue.DefaultQueue.AddTransactionObserver(queueObserver);
		}
		
		if (productRequestDelegate == null)
		{
			productRequestDelegate = new ExampleProductRequestDelegate();
			productRequestDelegate.Retain();
		}
		
		
		var productIds = new AKiOS.NSMutableSet();
		productIds.AddObject(AKiOS.NSString.StringWithUTF8String("com.jdshasgdj.product_id_1"));
		
		var productRequest = SKProductsRequest.AllocInitWithProductIdentifiers(productIds);
		productRequest.SetDelegate(productRequestDelegate);
		productRequest.Start();
	}
}
