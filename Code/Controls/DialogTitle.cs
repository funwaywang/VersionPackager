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
    public class DialogTitle : HeaderedContentControl
    {
        public static readonly DependencyProperty CloseButtonVisibilityProperty = DependencyProperty.Register("CloseButtonVisibility",
            typeof(Visibility), typeof(DialogTitle), new PropertyMetadata(Visibility.Visible));

        static DialogTitle()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogTitle), new FrameworkPropertyMetadata(typeof(DialogTitle)));
        }

        public DialogTitle()
        {
            CommandBindings.Add(new CommandBinding(Commands.ClosePopup, ClosePopup_Executed, ClosePopup_CanExecute));
        }

        public Visibility CloseButtonVisibility
        {
            get => (Visibility)GetValue(CloseButtonVisibilityProperty);
            set => SetValue(CloseButtonVisibilityProperty, value);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.ChangedButton == MouseButton.Left)
            {
                var window = Window.GetWindow(this);
                window.DragMove();
            }
        }

        void ClosePopup_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CloseButtonVisibility == Visibility.Visible;
        }

        void ClosePopup_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = this.TryFindAncestor<Window>();
            if (dialog != null)
            {
                dialog.Close();
            }
        }
    }
}
