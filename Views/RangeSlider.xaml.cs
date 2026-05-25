/*
*   FILE          : RangeSlider.xaml.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Code-behind for RangeSlider UserControl, handling
*                   thumb drag logic, range bar updates, value clamping,
*                   & buff tooltip visibility above 100.
*/
using D2ArmorCalc_Helpers;
using D2ArmorCalc_Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace D2ArmorCalc {
    public partial class RangeSlider : UserControl {
        //=====================================================================
        //Dependency Properties.
        //=====================================================================
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(nameof(MinValue), typeof(int), typeof(RangeSlider),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(nameof(MaxValue), typeof(int), typeof(RangeSlider),
                new FrameworkPropertyMetadata(200, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));
        public static readonly DependencyProperty StatProperty = DependencyProperty.Register(nameof(Stat), typeof(Stat), typeof(RangeSlider), new PropertyMetadata(Stat.Health));
        public int MinValue {get => (int)GetValue(MinValueProperty); set => SetValue(MinValueProperty, value);}
        public int MaxValue {get => (int)GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value);}
        public Stat Stat {get => (Stat)GetValue(StatProperty); set => SetValue(StatProperty, value);}
        //=====================================================================
        //Private State.
        //=====================================================================
        private bool _isDraggingMin;
        private bool _isDraggingMax;
        private bool _isUpdating;
        private double TrackWidth => Math.Max(0, ThumbCanvas.ActualWidth - 20);
        //=====================================================================
        //Constructor.
        //=====================================================================
        public RangeSlider(){
            InitializeComponent();

            Loaded += (s, e) => UpdateThumbPositions();

            MinThumb.MouseLeftButtonDown += (s, e) => {
                _isDraggingMin = true;
                MinThumb.CaptureMouse();
                e.Handled = true;
            };
            MinThumb.MouseLeftButtonUp += (s, e) => {
                _isDraggingMin = false;
                MinThumb.ReleaseMouseCapture();
            };
            MinThumb.MouseMove += (s, e) => {
                if (!_isDraggingMin) return;
                double pos = Math.Max(0, Math.Min(TrackWidth, e.GetPosition(ThumbCanvas).X - 10));
                int value = PositionToValue(pos);
                SetCurrentValue(MinValueProperty, Math.Min(value, MaxValue));
            };
            MinThumb.MouseEnter += (s, e) => MinThumb.Fill = (SolidColorBrush)FindResource("ThumbHoverBrush");
            MinThumb.MouseLeave += (s, e) => MinThumb.Fill = (SolidColorBrush)FindResource("ThumbBrush");
            MaxThumb.MouseLeftButtonDown += (s, e) => {
                _isDraggingMax = true;
                MaxThumb.CaptureMouse();
                e.Handled = true;
            };
            MaxThumb.MouseLeftButtonUp += (s, e) => {
                _isDraggingMax = false;
                MaxThumb.ReleaseMouseCapture();
            };
            MaxThumb.MouseMove += (s, e) => {
                if (!_isDraggingMax) return;
                double pos = Math.Max(0, Math.Min(TrackWidth, e.GetPosition(ThumbCanvas).X - 10));
                int value = PositionToValue(pos);
                SetCurrentValue(MaxValueProperty, Math.Max(value, MinValue));
            };
            MaxThumb.MouseEnter += (s, e) => MaxThumb.Fill = (SolidColorBrush)FindResource("ThumbHoverBrush");
            MaxThumb.MouseLeave += (s, e) => MaxThumb.Fill = (SolidColorBrush)FindResource("ThumbBrush");
        }
        //=====================================================================
        //Layout.
        //=====================================================================
        /*
        Method        : UpdateThumbPositions
        Description   : Recalculates & applies canvas positions for both
                        thumbs & updates range bar width & offset.
        Parameters    : None.
        Return Values : void
        */
        private void UpdateThumbPositions(){
            if (_isUpdating) return;
            if (TrackWidth <= 0) return;

            _isUpdating = true;
            double minPos = ValueToPosition(MinValue);
            double maxPos = ValueToPosition(MaxValue);

            Canvas.SetLeft(MinThumb, minPos);
            Canvas.SetLeft(MaxThumb, maxPos);

            double barLeft = minPos + 10;
            double barWidth = Math.Max(0, maxPos - minPos);

            RangeBar.Margin = new Thickness(barLeft, 0, 0, 0);
            RangeBar.Width = barWidth;

            UpdateTooltips();

            _isUpdating = false;
        }
        /*
        Method        : UpdateTooltips
        Description   : Updates tooltip content for both thumbs. 
                        Tooltips only show when thumb value exceeds 100.
        Parameters    : None.
        Return Values : void
        */
        private void UpdateTooltips(){
            string minBuff = StatHelper.GetBuff(Stat, MinValue);
            string maxBuff = StatHelper.GetBuff(Stat, MaxValue);

            MinThumb.ToolTip = string.IsNullOrEmpty(minBuff) ? null : (object)minBuff;
            MaxThumb.ToolTip = string.IsNullOrEmpty(maxBuff) ? null : (object)maxBuff;
        }
        //=====================================================================
        //Value / Position Conversion
        //=====================================================================
        /*
        Method        : ValueToPosition
        Description   : Converts stat value (0-200) to canvas X position.
        Parameters    : int value : Stat value to convert.
        Return Values : double    : X position on canvas.
        */
        private double ValueToPosition(int value){
            return (value / 200.0) * TrackWidth;
        }
        /*
        Method        : PositionToValue
        Description   : Converts canvas X position to stat value (0-200).
        Parameters    : double position : Canvas X position.
        Return Values : int             : Clamped stat value.
        */
        private int PositionToValue(double position){
            int value = (int)Math.Round((position / TrackWidth) * 200.0);
            value = (int)Math.Round(value / 5.0) * 5; //snap to nearest 5.
            return Math.Max(0, Math.Min(200, value));
        }
        //=====================================================================
        //Callbacks.
        //=====================================================================
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e){
            if (d is RangeSlider slider && !slider._isUpdating) slider.UpdateThumbPositions();
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo info){
            base.OnRenderSizeChanged(info);
            UpdateThumbPositions();
        }
    }
}