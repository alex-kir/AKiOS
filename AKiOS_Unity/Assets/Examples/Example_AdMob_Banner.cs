using System;
using AKiOS;
using UnityEngine;

public class Example_AdMob_Banner : Example0
{
	class ExampleOfBannerDelegate : AKiOS.Core.ProxyObject
	{
		public ExampleOfBannerDelegate()
		{
			InitProxyObject("alloc", "init"); // requered for objects derived from ProxyObject
		}
		
		[AKiOS.Core.CocoaMethod("adViewDidReceiveAd:")]
		public void adViewDidReceiveAd_(GADBannerView adView)
		{
			LOG("adViewDidReceiveAd_");
			adView.Hidden = false;
		}
		
		[AKiOS.Core.CocoaMethod("adView:didFailToReceiveAdWithError:")]
		public void adView_didFailToReceiveAdWithError_(GADBannerView adView, NSError error)
		{
			LOG("adView_didFailToReceiveAdWithError_");
			LOG(error.LocalizedDescription.ToString());
			adView.RemoveFromSuperview();
		}
	}
	
	string admobId = "a151f6714174302";
	
	protected override void OnRun()
	{
		float bannerWidth =  320;
		float bannerHeight = 50;
		if (iPhone.generation.ToString().StartsWith("iPad"))
		{
			bannerWidth =  728;
			bannerHeight = 90;
		}
		
		// --------------------
		
		var keyWindow = UIApplication.SharedApplication.KeyWindow;
		var viewController = keyWindow.RootViewController;
		var unityView = keyWindow.Subviews.ObjectAtIndex(0);
	
		//var sz = new GADAdSize { Size_Width = 300, Size_Height = 250, Flags = 0 };
		//var sz = new GADAdSize { Size_Width = 320, Size_Height = 50, Flags = 0 };
		var sz = new GADAdSize { Size_Width = bannerWidth, Size_Height = bannerHeight, Flags = 0 };
		var currentBannerView = GADBannerView.AllocInitWithAdSize(sz);
		if (currentBannerView.IsNil)
		{
			LOG("https://developers.google.com/mobile-ads-sdk/docs/#ios");
			LOG(" - Other Linker Flags: -ObjC");
			LOG(" - Frameworks:");
            LOG("    - AdSupport");
            LOG("    - AudioToolbox");
            LOG("    - AVFoundation");
            LOG("    - CoreGraphics");
            LOG("    - MessageUI");
            LOG("    - StoreKit");
            LOG("    - SystemConfiguration");
		}
		
		unityView.AddSubview(currentBannerView);
		
		currentBannerView.RootViewController = viewController;
		currentBannerView.AdUnitID = NSString.StringWithUTF8String(admobId);

		var bannerDelegte = new ExampleOfBannerDelegate();
		currentBannerView.Delegate = bannerDelegte;
		
		var request = GADRequest.New();
		var testDevices = AKiOS.NSMutableArray<NSString>.Array();
		testDevices.AddObject(NSString.StringWithUTF8String("1cf5076f1d9f0cd99458fd28e29dc3ea"));
		request.TestDevices = testDevices;
		currentBannerView.LoadRequest(request);
		
		float screenScale = AKiOS.UIScreen.MainScreen.Scale;
		currentBannerView.Center = new CGPoint() { x = Screen.width / 2 / screenScale, y = bannerHeight / 2 };
		currentBannerView.Hidden = true;
	}
}

