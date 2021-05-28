using Android.Views;
using Android.Widget;
using AView = Android.Views.View;

namespace Microsoft.Maui
{
	public static class ViewExtensions
	{
		const int DefaultAutomationTagId = -1;
		public static int AutomationTagId { get; set; } = DefaultAutomationTagId;

		public static void UpdateIsEnabled(this AView nativeView, IView view)
		{
			if (nativeView != null)
				nativeView.Enabled = view.IsEnabled;
		}

		public static void UpdateBackgroundColor(this AView nativeView, IView view)
		{
			var backgroundColor = view.BackgroundColor;
			if (!backgroundColor.IsDefault)
				nativeView?.SetBackgroundColor(backgroundColor.ToNative());
		}

		public static bool GetClipToOutline(this AView view)
		{
			return view.ClipToOutline;
		}

		public static void SetClipToOutline(this AView view, bool value)
		{
			view.ClipToOutline = value;
		}

		public static void UpdateAutomationId(this AView nativeView, IView view)
		{
			if (AutomationTagId == DefaultAutomationTagId)
			{
				AutomationTagId = Resource.Id.automation_tag_id;
			}

			nativeView.SetTag(AutomationTagId, view.AutomationId);
		}

		public static void UpdateShadow(this AView nativeView, IView view)
		{
			var radius = view.Shadow.Radius;

			if (radius < 0)
				return;

			var opacity = view.Shadow.Opacity;

			if (opacity < 0)
				return;

			var nativeColor = view.Shadow.Color.ToNative();

			if (nativeView is TextView textView)
			{
				var offsetX = (float)view.Shadow.Offset.Width;
				var offsetY = (float)view.Shadow.Offset.Height;
				textView.SetShadowLayer(radius, offsetX, offsetY, nativeColor);
				return;
			}

			nativeView.OutlineProvider = view.BackgroundColor.A > 0
				? ViewOutlineProvider.PaddedBounds
				: ViewOutlineProvider.Bounds;

			if (nativeView.Context != null)
				nativeView.Elevation = nativeView.Context.ToPixels(radius);

			if (!NativeVersion.Supports(NativeApis.OutlineAmbientShadowColor))
				return;

			nativeView.SetOutlineAmbientShadowColor(nativeColor);
			nativeView.SetOutlineSpotShadowColor(nativeColor);
		}
	}
}