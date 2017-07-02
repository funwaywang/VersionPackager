using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace VPackager
{
    public class SplitButton : Button
    {
        public static readonly DependencyProperty DropDownMenuProperty = DependencyProperty.Register("DropDownMenu", typeof(ContextMenu), typeof(SplitButton));
        public static readonly DependencyProperty IconTextProperty = DependencyProperty.Register("IconText", typeof(string), typeof(SplitButton));
        public static readonly DependencyProperty IconBrushProperty = DependencyProperty.Register("IconBrush", typeof(Brush), typeof(SplitButton));
        public static readonly DependencyProperty IsDroppedProperty = DependencyProperty.Register("IsDropped", typeof(bool), typeof(SplitButton));

        static SplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));
        }

        public ContextMenu DropDownMenu
        {
            get => (ContextMenu)GetValue(DropDownMenuProperty);
            set => SetValue(DropDownMenuProperty, value);
        }

        public string IconText
        {
            get => (string)GetValue(IconTextProperty);
            set => SetValue(IconTextProperty, value);
        }

        public Brush IconBrush
        {
            get => (Brush)GetValue(IconBrushProperty);
            set => SetValue(IconBrushProperty, value);
        }

        public bool IsDropped
        {
            get => (bool)GetValue(IsDroppedProperty);
            set => SetValue(IsDroppedProperty, value);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if(e.Property == DropDownMenuProperty)
            {
                OnDropDownMenuChanged(e);
            }
        }

        private void OnDropDownMenuChanged(DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue is ContextMenu menu)
            {
                menu.PlacementTarget = this;
                menu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                menu.SetBinding(ContextMenu.IsOpenProperty, new Binding(IsDroppedProperty.Name) { Source = this, Mode = BindingMode.TwoWay });
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if(GetTemplateChild("DropDownButton") is Button button)
            {
                button.Click += Button_Click;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IsDropped = !IsDropped;
        }
    }
}
