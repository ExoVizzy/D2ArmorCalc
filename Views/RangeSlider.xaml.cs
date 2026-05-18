/*
*   FILE          : RangeSlider.xaml.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Code-behind for RangeSlider UserControl, handling
*                   thumb drag logic, range bar updates, value clamping,
*                   & buff tooltip visibility above 100.
*/
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace D2ArmorCalc {
    public partial class RangeSlider : UserControl {
        //=====================================================================
        //Dependency Properties.
        //=====================================================================
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(nameof(MinValue), typeof(int), typeof(RangeSlider),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(nameof(MaxValue), typeof(int), typeof(RangeSlider),
                new FrameworkPropertyMetadata(200, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));
        public static readonly DependencyProperty RangeMinProperty =
            DependencyProperty.Register(nameof(RangeMin), typeof(int), typeof(RangeSlider),
                new PropertyMetadata(0, OnValueChanged));
        public static readonly DependencyProperty RangeMaxProperty =
            DependencyProperty.Register(nameof(RangeMax), typeof(int), typeof(RangeSlider),
                new PropertyMetadata(200, OnValueChanged));
        public static readonly DependencyProperty StatProperty =
            DependencyProperty.Register(nameof(Stat), typeof(Stat), typeof(RangeSlider),
                new PropertyMetadata(Stat.Health));
        public int MinValue {get => (int)GetValue(MinValueProperty); set => SetValue(MinValueProperty, value);}
        public int MaxValue {get => (int)GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value);}
        public int RangeMin {get => (int)GetValue(RangeMinProperty); set => SetValue(RangeMinProperty, value);}
        public int RangeMax {get => (int)GetValue(RangeMaxProperty); set => SetValue(RangeMaxProperty, value);}
        public Stat Stat {get => (Stat)GetValue(StatProperty); set => SetValue(StatProperty, value);}
        //=====================================================================
        //Private State.
        //=====================================================================
        private bool _isDraggingMin;
        private bool _isDraggingMax;
        private double _trackWidth => ThumbCanvas.ActualWidth - 20; //subtract thumb width
        //=====================================================================
        //Constructor.
        //=====================================================================
        public RangeSlider(){
            InitializeComponent();
            Loaded += (s, e) => UpdateLayout();

            MinThumb.MouseLeftButtonDown += MinThumb_MouseDown;
            MinThumb.MouseLeftButtonUp += MinThumb_MouseUp;
            MinThumb.MouseMove += MinThumb_MouseMove;
            MinThumb.MouseEnter += (s, e) => MinThumb.Fill = (SolidColorBrush)FindResource("ThumbHoverBrush");
            MinThumb.MouseLeave += (s, e) => MinThumb.Fill = (SolidColorBrush)FindResource("ThumbBrush");

            MaxThumb.MouseLeftButtonDown += MaxThumb_MouseDown;
            MaxThumb.MouseLeftButtonUp += MaxThumb_MouseUp;
            MaxThumb.MouseMove += MaxThumb_MouseMove;
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
            if (_trackWidth <= 0) return;

            double minPos = ValueToPosition(MinValue);
            double maxPos = ValueToPosition(MaxValue);

            Canvas.SetLeft(MinThumb, minPos);
            Canvas.SetLeft(MaxThumb, maxPos);

            //Range bar starts at min thumb center, ends at max thumb center.
            double barLeft = minPos + 10;
            double barWidth = maxPos - minPos;

            RangeBar.Margin = new Thickness(barLeft, 0, 0, 0);
            RangeBar.Width = Math.Max(0, barWidth);

            UpdateTooltips();
        }
        /*
        Method        : UpdateTooltips
        Description   : Updates tooltip content for both thumbs. Tooltips only
                        show buff text when thumb value exceeds 100.
        Parameters    : None.
        Return Values : void
        */
        private void UpdateTooltips(){
            string minBuff = StatHelper.GetBuff(Stat, MinValue);
            string maxBuff = StatHelper.GetBuff(Stat, MaxValue);

            MinBuffText.Text = minBuff;
            MinThumb.ToolTip = string.IsNullOrEmpty(minBuff) ? null : MinThumb.ToolTip;

            MaxBuffText.Text = maxBuff;
            MaxThumb.ToolTip = string.IsNullOrEmpty(maxBuff) ? null : MaxThumb.ToolTip;
        }
        //=====================================================================
        //Value / Position Conversion.
        //=====================================================================
        /*
        Method        : ValueToPosition
        Description   : Converts stat value (0-200) to canvas X position.
        Parameters    : int value : Stat value to convert.
        Return Values : double    : X position on canvas.
        */
        private double ValueToPosition(int value){
            return (value / 200.0) * _trackWidth;
        }
        /*
        Method        : PositionToValue
        Description   : Converts canvas X position to stat value (0-200).
        Parameters    : double position : Canvas X position.
        Return Values : int             : Clamped stat value.
        */
        private int PositionToValue(double position){
            int value = (int)Math.Round((position / _trackWidth) * 200.0);
            return Math.Max(0, Math.Min(200, value));
        }
        //=====================================================================
        //Dependency Property Callback.
        //=====================================================================
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e){
            if (d is RangeSlider slider) slider.UpdateThumbPositions();
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo info){
            base.OnRenderSizeChanged(info);
            UpdateThumbPositions();
        }
        //=====================================================================
        //Min Thumb Events.
        //=====================================================================
        private void MinThumb_MouseDown(object sender, MouseButtonEventArgs e){
            _isDraggingMin = true;
            MinThumb.CaptureMouse();
            e.Handled = true;
        }
        private void MinThumb_MouseUp(object sender, MouseButtonEventArgs e){
            _isDraggingMin = false;
            MinThumb.ReleaseMouseCapture();
        }
        private void MinThumb_MouseMove(object sender, MouseEventArgs e){
            if (!_isDraggingMin) return;
            double pos = e.GetPosition(ThumbCanvas).X - 10;
            int value = PositionToValue(pos);
            MinValue = Math.Min(value, MaxValue);
        }
        //=====================================================================
        //Max Thumb Events.
        //=====================================================================
        private void MaxThumb_MouseDown(object sender, MouseButtonEventArgs e){
            _isDraggingMax = true;
            MaxThumb.CaptureMouse();
            e.Handled = true;
        }
        private void MaxThumb_MouseUp(object sender, MouseButtonEventArgs e){
            _isDraggingMax = false;
            MaxThumb.ReleaseMouseCapture();
        }
        private void MaxThumb_MouseMove(object sender, MouseEventArgs e){
            if (!_isDraggingMax) return;
            double pos = e.GetPosition(ThumbCanvas).X - 10;
            int value = PositionToValue(pos);
            MaxValue = Math.Max(value, MinValue);
        }
    }
}