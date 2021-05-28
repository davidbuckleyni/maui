using System.Collections.Generic;
using CoreGraphics;
using UIKit;

namespace Microsoft.Maui
{
	public static class ViewExtensions
	{
		public static UIColor? GetBackgroundColor(this UIView view)
			=> view?.BackgroundColor;

		public static void UpdateIsEnabled(this UIView nativeView, IView view)
		{
			if (!(nativeView is UIControl uiControl))
				return;

			uiControl.Enabled = view.IsEnabled;
		}

		public static void UpdateBackgroundColor(this UIView nativeView, IView view)
		{
			if (nativeView == null)
				return;

			var color = view.BackgroundColor;

			if (!color.IsDefault)
				nativeView.BackgroundColor = color.ToNative();
		}

		public static void UpdateAutomationId(this UIView nativeView, IView view) =>
			nativeView.AccessibilityIdentifier = view.AutomationId;

		public static T? FindDescendantView<T>(this UIView view) where T : UIView
		{
			var queue = new Queue<UIView>();
			queue.Enqueue(view);

			while (queue.Count > 0)
			{
				var descendantView = queue.Dequeue();

				if (descendantView is T result)
					return result;

				for (var i = 0; i < descendantView.Subviews?.Length; i++)
					queue.Enqueue(descendantView.Subviews[i]);
			}

			return null;
		}

		public static void UpdateShadow(this UIView nativeView, IView view)
		{
			var radius = view.Shadow.Radius;

			if (radius < 0)
				return;

			var opacity = view.Shadow.Opacity;

			if (opacity < 0)
				return;

			var nativeColor = view.Shadow.Color.ToNative();

			nativeView.Layer.ShadowColor = nativeColor.CGColor;
			nativeView.Layer.ShadowOpacity = opacity;
			nativeView.Layer.ShadowRadius = radius;
			nativeView.Layer.ShadowOffset = new CGSize((double)view.Shadow.Offset.Width, (double)view.Shadow.Offset.Height);
		}
	}
}