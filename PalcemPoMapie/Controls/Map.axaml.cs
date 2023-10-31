using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Diagnostics;

namespace PalcemPoMapie.Controls
{
    [TemplatePart("PART_ScrollViewer", typeof(ScrollViewer))]
    [TemplatePart("PART_MapBorder", typeof(Border))]
    [TemplatePart("PART_MapImage", typeof(Image))]
    public partial class Map : UserControl
    {
        private Point _lastPointerPosition = new();
        private ScrollViewer? _scrollViewer;
        private Border? _mapBorder;
        private Image? _mapImage;

        public static readonly StyledProperty<Vector> OffsetProperty =
            AvaloniaProperty.Register<Map, Vector>(nameof(Offset));

        public static readonly StyledProperty<double> ScaleProperty =
            AvaloniaProperty.Register<Map, double>(nameof(Scale), defaultValue: 1);

        public static readonly StyledProperty<double> MaxScaleProperty =
            AvaloniaProperty.Register<Map, double>(nameof(MaxScale), defaultValue: 10);

        public static readonly StyledProperty<double> MinScaleProperty =
            AvaloniaProperty.Register<Map, double>(nameof(MinScale), defaultValue: 0.1);

        public static readonly StyledProperty<double> ZoomInFactorProperty =
            AvaloniaProperty.Register<Map, double>(nameof(ZoomInFactor), defaultValue: 1.1);

        public static readonly StyledProperty<double> ZoomOutFactorProperty =
            AvaloniaProperty.Register<Map, double>(nameof(ZoomOutFactor), defaultValue: 0.9);

        public static readonly StyledProperty<IImage> ImageSourceProperty =
            AvaloniaProperty.Register<Map, IImage>(nameof(ImageSource));

        public Vector Offset
        {
            get => GetValue(OffsetProperty);
            set => SetValue(OffsetProperty, value);
        }

        public double Scale
        {
            get => GetValue(ScaleProperty);
            set => SetValue(ScaleProperty, value);
        }

        public double MaxScale
        {
            get => GetValue(MaxScaleProperty);
            set => SetValue(MaxScaleProperty, value);
        }

        public double MinScale
        {
            get => GetValue(MinScaleProperty);
            set => SetValue(MinScaleProperty, value);
        }

        public double ZoomInFactor
        {
            get => GetValue(ZoomInFactorProperty);
            set => SetValue(ZoomInFactorProperty, value);
        }

        public double ZoomOutFactor
        {
            get => GetValue(ZoomOutFactorProperty);
            set => SetValue(ZoomOutFactorProperty, value);
        }

        public IImage ImageSource
        {
            get => GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public event EventHandler<MapScaleChangedEventArgs>? ScaleChanged;

        public Map()
        {
            InitializeComponent();
        }

        public void ZoomToScale(double targetScale, PointerEventArgs args) => ZoomToScale(targetScale, args.GetPosition(_mapBorder));

        public void ZoomToScale(double targetScale, Point pointerPosition) => ZoomByFactor(targetScale / Scale, pointerPosition);

        public void ZoomByFactor(double zoomingFactor, PointerEventArgs args) => ZoomByFactor(zoomingFactor, args.GetPosition(_mapBorder));

        public void ZoomByFactor(double zoomingFactor, Point pointerPosition)
        {
            zoomingFactor = SetScaleByDelta(zoomingFactor);

            Vector newVector = new(
                Offset.X + zoomingFactor * (pointerPosition.X - Padding.Left) + Padding.Left - pointerPosition.X,
                Offset.Y + zoomingFactor * (pointerPosition.Y - Padding.Top) + Padding.Top - pointerPosition.Y);

            // fix for bug ^^
            Offset = newVector;
            Offset = newVector;
        }

        public void Pan(PointerEventArgs e)
        {
            Point currentPoinerPosition = e.GetPosition(_scrollViewer);
            Vector newOffset = Offset - (currentPoinerPosition - _lastPointerPosition);
            newOffset = new Vector(
                Math.Min(Math.Max(0, newOffset.X), _scrollViewer.ScrollBarMaximum.X),
                Math.Min(Math.Max(0, newOffset.Y), _scrollViewer.ScrollBarMaximum.Y));

            Offset = newOffset;
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            _scrollViewer = e.NameScope.Get<ScrollViewer>("PART_ScrollViewer");
            _mapImage = e.NameScope.Get<Image>("PART_MapImage");
            _mapBorder = e.NameScope.Get<Border>("PART_MapBorder");
            
            _mapImage.PointerWheelChanged += WheelChanged;
            _mapBorder.PointerWheelChanged += WheelChanged;
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == ScaleProperty &&
                change.NewValue is double newValue &&
                change.OldValue is double oldValue)
            {
                MapScaleChangedEventArgs eventArgs = new(newValue, oldValue);
                ScaleChanged?.Invoke(this, eventArgs);
                
                if (!eventArgs.SupressImageScale)
                {
                    ScaleImage(eventArgs);
                }

                if (!eventArgs.SupressContentScale)
                {
                    ScaleContent();
                }
            }
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);

            if (e.Handled is false)
            {
                if (e.GetCurrentPoint(_scrollViewer).Properties.IsRightButtonPressed && _scrollViewer is { })
                {
                    Pan(e);
                }
            }

            _lastPointerPosition = e.GetPosition(_scrollViewer);
        }

        protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
            base.OnPointerWheelChanged(e);
            if (e.Handled is false)
            {
                double deltaScale = e.Delta.Y switch
                {
                    0 => 1,
                    > 0 => ZoomInFactor,
                    < 0 => ZoomOutFactor,
                    _ => throw new NotImplementedException(),
                };

                ZoomByFactor(deltaScale, e);
            }
        }

        private void WheelChanged(object? sender, PointerWheelEventArgs e)
        {
            OnPointerWheelChanged(e);
            e.Handled = true;
        }

        private double SetScaleByDelta(double deltaScale)
        {
            if (Scale * deltaScale > MaxScale)
            {
                deltaScale = MaxScale / Scale;
                Scale = MaxScale;
                return deltaScale;
            }

            if (Scale * deltaScale < MinScale)
            {
                deltaScale = MinScale / Scale;
                Scale = MinScale;
                return deltaScale;
            }

            Scale *= deltaScale;
            return deltaScale;
        }

        private void ScaleImage(MapScaleChangedEventArgs e)
        {
            if (_mapImage is { })
            {
                _mapImage.Width = Math.Round(_mapImage.Bounds.Width * (e.NewScale / e.OldScale));
                _mapImage.Height = Math.Round(_mapImage.Bounds.Height * (e.NewScale / e.OldScale));
            }
        }

        private void ScaleContent()
        {
            if (Content is Visual content)
            {
                content.RenderTransform = new ScaleTransform(Scale, Scale);
            }
        }
    }
}
