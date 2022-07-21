using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace IconButtonControl
{
    public class IconButton : Control
    {
        public SolidColorBrush ColorBrush
        {
            get { return (SolidColorBrush)GetValue(ColorBrushProperty); }
            set { SetValue(ColorBrushProperty, value); }
        }
        public static readonly DependencyProperty ColorBrushProperty =
            DependencyProperty.Register("ColorBrush", typeof(SolidColorBrush),
                typeof(IconButton), new PropertyMetadata(new SolidColorBrush(Colors.Black)));


        public SolidColorBrush HoverBrush
        {
            get { return (SolidColorBrush)GetValue(HoverBrushProperty); }
            set { SetValue(HoverBrushProperty, value); }
        }
        public static readonly DependencyProperty HoverBrushProperty =
            DependencyProperty.Register("HoverBrush", typeof(SolidColorBrush),
                typeof(IconButton), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));


        public SolidColorBrush DisabledBrush
        {
            get { return (SolidColorBrush)GetValue(DisabledBrushProperty); }
            set { SetValue(DisabledBrushProperty, value); }
        }
        public static readonly DependencyProperty DisabledBrushProperty =
            DependencyProperty.Register("DisabledBrush", typeof(SolidColorBrush),
                typeof(IconButton), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));


        public SolidColorBrush UncheckedBrush
        {
            get { return (SolidColorBrush)GetValue(UncheckedBrushProperty); }
            set { SetValue(UncheckedBrushProperty, value); }
        }
        public static readonly DependencyProperty UncheckedBrushProperty =
            DependencyProperty.Register("UncheckedBrush", typeof(SolidColorBrush),
                typeof(IconButton), new PropertyMetadata(new SolidColorBrush(Colors.Red)));


        public SolidColorBrush IsCheckedBrush
        {
            get { return (SolidColorBrush)GetValue(IsCheckedBrushProperty); }
            set { SetValue(IsCheckedBrushProperty, value); }
        }
        public static readonly DependencyProperty IsCheckedBrushProperty =
            DependencyProperty.Register("IsCheckedBrush", typeof(SolidColorBrush),
                typeof(IconButton), new PropertyMetadata(new SolidColorBrush(Colors.Green)));


        public Geometry Icon
        {
            get { return (Geometry)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(Geometry),
                typeof(IconButton), new PropertyMetadata(null));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand),
                typeof(IconButton), new PropertyMetadata(null));


        public ICommand CheckedCommand
        {
            get { return (ICommand)GetValue(CheckedCommandProperty); }
            set { SetValue(CheckedCommandProperty, value); }
        }
        public static readonly DependencyProperty CheckedCommandProperty =
            DependencyProperty.Register("CheckedCommand", typeof(ICommand), 
                typeof(IconButton), new PropertyMetadata(null));


        public ICommand UncheckedCommand
        {
            get { return (ICommand)GetValue(UncheckedCommandProperty); }
            set { SetValue(UncheckedCommandProperty, value); }
        }
        public static readonly DependencyProperty UncheckedCommandProperty =
            DependencyProperty.Register("UncheckedCommand", typeof(ICommand),
                typeof(IconButton), new PropertyMetadata(null));


        static IconButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconButton), new FrameworkPropertyMetadata(typeof(IconButton)));
        }


        public bool IsToggle
        {
            get { return (bool)GetValue(IsToggleProperty); }
            set { SetValue(IsToggleProperty, value); }
        }
        public static readonly DependencyProperty IsToggleProperty =
            DependencyProperty.Register("IsToggle", typeof(bool), typeof(IconButton),
                new PropertyMetadata(false, IsToggleChanged));

        private static void IsToggleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is IconButton) ((IconButton)d).OnIsToggleChanged();
        }

        public void OnIsToggleChanged()
        {
            if (IsToggle)
            {
                if (IsEnabled) Foreground = IsChecked ? IsCheckedBrush : UncheckedBrush;
                else Foreground = DisabledBrush;
            }
            else
            {
                if(IsEnabled) Foreground = IsMouseOver ? HoverBrush : ColorBrush;
                else Foreground = DisabledBrush;
            }
        }


        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool),
                typeof(IconButton), new PropertyMetadata(false, IsCheckedChanged));

        private static void IsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is IconButton) ((IconButton)d).OnIsCheckedChanged();
        }

        public void OnIsCheckedChanged()
        {
            if(IsToggle)
            {
                Foreground = IsChecked ? IsCheckedBrush : UncheckedBrush;

                if (IsChecked) CheckedCommand?.Execute(null);
                else UncheckedCommand?.Execute(null);
            }
        }

        public bool HoverEnabled
        {
            get { return (bool)GetValue(HoverEnabledProperty); }
            set { SetValue(HoverEnabledProperty, value); }
        }
        public static readonly DependencyProperty HoverEnabledProperty =
            DependencyProperty.Register("HoverEnabled", typeof(bool), 
                typeof(IconButton), new PropertyMetadata(true));


        public bool AnimationsEnabled
        {
            get { return (bool)GetValue(AnimationsEnabledProperty); }
            set { SetValue(AnimationsEnabledProperty, value); }
        }
        public static readonly DependencyProperty AnimationsEnabledProperty =
            DependencyProperty.Register("AnimationsEnabled", typeof(bool),
                typeof(IconButton), new PropertyMetadata(true));

        private DoubleAnimation scaleDownAnim;
        private DoubleAnimation scaleUpAnim;
        private ScaleTransform? scale;

        public IconButton()
        {
            scaleDownAnim = new DoubleAnimation();
            scaleDownAnim.To = 0.95;
            scaleDownAnim.Duration = new Duration(TimeSpan.FromMilliseconds(100));

            scaleUpAnim = new DoubleAnimation();
            scaleUpAnim.To = 1;
            scaleUpAnim.Duration = new Duration(TimeSpan.FromMilliseconds(100));

            Loaded += IB_Loaded;
        }

        private void IB_Loaded(object sender, RoutedEventArgs e)
        {
            var template = this.Template;
            scale = (ScaleTransform)template.FindName("iconScale", this);

            if(IsToggle)
            {
                if(IsEnabled) Foreground = IsChecked ? IsCheckedBrush : UncheckedBrush;
                else Foreground = DisabledBrush;
            }
            else Foreground = IsEnabled ? DisabledBrush : ColorBrush;

            IsEnabledChanged += IB_IsEnabledChanged;
        }

        private void IB_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsToggle)
            {
                if (IsEnabled) Foreground = IsChecked ? IsCheckedBrush : UncheckedBrush;
                else Foreground = DisabledBrush;
            }
            else Foreground = IsEnabled ? DisabledBrush : ColorBrush;
        }


        private void AnimateOut()
        {
            if (scale != null)
            {
                scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleUpAnim);
                scale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleUpAnim);
            }
        }

        private void AnimateIn()
        {
            if (scale != null)
            {
                scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleDownAnim);
                scale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleDownAnim);
            }
        }


        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (AnimationsEnabled) AnimateIn();

            if (IsToggle) IsChecked = !IsChecked;

            Command?.Execute(null);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (AnimationsEnabled) AnimateOut();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if(HoverEnabled && !IsToggle) Foreground = HoverBrush;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (AnimationsEnabled) AnimateOut();

            if(HoverEnabled && !IsToggle) Foreground = ColorBrush;
        }
    }
}