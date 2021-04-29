using System;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Google.Android.Material.AppBar;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Graphics;
using AView = Android.Views.View;
using LP = Android.Views.ViewGroup.LayoutParams;

namespace Microsoft.Maui.Controls.Platform
{
	// This is used to monitor an xplat View and apply layout changes
	internal class ShellContentView
	{
		public IViewHandler Handler { get; private set; }
		View _view;
		WeakReference<Context> _context;
		readonly IMauiContext _mauiContext;
		IView MauiView => View;

		// These are used by layout calls made by android if the layouts
		// are invalidated. This ensures that the layout is performed
		// using the same input values
		public double Width { get; private set; }
		public double Height { get; private set; }
		public double? MaxWidth { get; private set; }
		public double? MaxHeight { get; private set; }
		public double X { get; private set; }
		public double Y { get; private set; }

		public ShellContentView(Context context, View view, IMauiContext mauiContext)
		{
			_mauiContext = mauiContext ?? throw new ArgumentNullException(nameof(mauiContext));
			_context = new WeakReference<Context>(context);
			View = view;
		}

		public View View
		{
			get { return _view; }
			set
			{
				OnViewSet(value);
			}
		}

		public void TearDown()
		{
			View = null;
			Handler = null;
			_view = null;
			_context = null;
		}

		public void LayoutView(double x, double y, double width, double height, double? maxWidth = null, double? maxHeight = null)
		{
			if (width == -1)
				width = double.PositiveInfinity;

			if (height == -1)
				height = double.PositiveInfinity;

			Width = width;
			Height = height;
			MaxWidth = maxWidth;
			MaxHeight = maxHeight;
			X = x;
			Y = y;

			Context context;

			if (Handler == null || !(_context.TryGetTarget(out context)) || !NativeView.IsAlive())
				return;

			if (View == null)
			{
				MauiView.Measure(0, 0);
				MauiView.Arrange(Rectangle.Zero);
				return;
			}

			var request = MauiView.Measure(width, height);

			var layoutParams = NativeView.LayoutParameters;
			if (double.IsInfinity(height))
				height = request.Height;

			if (double.IsInfinity(width))
				width = request.Width;

			if (height > maxHeight)
				height = maxHeight.Value;

			if (width > maxWidth)
				width = maxWidth.Value;

			if (layoutParams.Width != LP.MatchParent)
				layoutParams.Width = (int)context.ToPixels(width);

			if (layoutParams.Height != LP.MatchParent)
				layoutParams.Height = (int)context.ToPixels(height);

			NativeView.LayoutParameters = layoutParams;
			MauiView.Arrange(new Rectangle(x, y, width, height));
			//Renderer.UpdateLayout();
		}

		public virtual void OnViewSet(View view)
		{
			if (View != null)
				View.SizeChanged -= OnViewSizeChanged;

			if (View is VisualElement oldView)
				oldView.MeasureInvalidated -= OnViewSizeChanged;

			if (View != null)
			{
				NativeView.RemoveFromParent();
				View.Handler = null;
			}

			_view = view;
			if (view != null)
			{
				Context context;

				if (!(_context.TryGetTarget(out context)))
					return;

				NativeView = view.ToNative(_mauiContext);
				Handler = view.Handler;

				if (View is VisualElement ve)
					ve.MeasureInvalidated += OnViewSizeChanged;
				else
					View.SizeChanged += OnViewSizeChanged;
			}
			else
			{
				NativeView = null;
			}
		}

		void OnViewSizeChanged(object sender, EventArgs e) =>
			LayoutView(X, Y, Width, Height, MaxWidth, MaxHeight);

		public AView NativeView
		{
			get;
			private set;
		}
	}
}