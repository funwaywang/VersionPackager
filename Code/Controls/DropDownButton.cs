using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VPackager
{
    public class DropDownButton : Button
    {
        public static readonly DependencyProperty DropDownMenuProperty = DependencyProperty.Register("DropDownMenu", typeof(ContextMenu), typeof(DropDownButton));
        public static readonly DependencyProperty OwnerElementProperty = DependencyProperty.Register("OwnerElement", typeof(FrameworkElement), typeof(DropDownButton));

        public ContextMenu DropDownMenu
        {
            get { return (ContextMenu)GetValue(DropDownMenuProperty); }
            set { SetValue(DropDownMenuProperty, value); }
        }

        public FrameworkElement OwnerElement
        {
            get { return (FrameworkElement)GetValue(OwnerElementProperty); }
            set { SetValue(OwnerElementProperty, value); }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if(DropDownMenu != null)
            {
                if(DropDownMenu.IsOpen)
                {
                    DropDownMenu.IsOpen = false;
                }
                else
                {
                    DropDownMenu.PlacementTarget = OwnerElement ?? this;
                    DropDownMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                    DropDownMenu.IsOpen = true;
                }
            }
        }
    }
}
