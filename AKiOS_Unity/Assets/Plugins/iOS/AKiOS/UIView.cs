using System;

namespace AKiOS
{
	public class UIView : NSObject
	{
		public NSArray<UIView> Subviews { get { return this.Call("subviews").Cast<NSArray<UIView>>(); } }

		public CGPoint Center {
			get { return this.Call("center").AsStruct<CGPoint>(); }
			set { this.Call("setCenter:", value); }
		}

		public bool Hidden {
			get { return this.Call("hidden").AsBool(); }
			set { this.Call("setHidden:", value); }
		}
		
		public void AddSubview(UIView subview)
		{
			this.Call("addSubview:", subview);
		}
		
		public void RemoveFromSuperview()
		{
			this.Call("removeFromSuperview");
		}
		
	}
}

