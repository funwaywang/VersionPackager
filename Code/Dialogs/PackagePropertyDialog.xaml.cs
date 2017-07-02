using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VPackager
{
    public partial class PackagePropertyDialog : Window
    {
        public static readonly DependencyProperty PackageProperty = DependencyProperty.Register("Package", typeof(Package), typeof(PackagePropertyDialog));

        public PackagePropertyDialog()
        {
            InitializeComponent();

            InitializeVersionPatterns();
        }

        public Package Package
        {
            get => (Package)GetValue(PackageProperty);
            set => SetValue(PackageProperty, value);
        }

        public PackageViewModel Model { get; private set; } = new PackageViewModel();

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if(e.Property == PackageProperty)
            {
                Model.LoadFrom(e.NewValue as Package);
            }
        }

        private void InitializeVersionPatterns()
        {
            var patterns = new Dictionary<string, string>
            {
                { "{YM}", Lang.GetText("YearMonth") },
                { "{MD}", Lang.GetText("MonthDay") },
                { "{HM}", Lang.GetText("HourMinute") },
                { "{YY}", Lang.GetText("Year") },
                { "{MM}", Lang.GetText("Month") },
                { "{DD}", Lang.GetText("Day") },
                { "{HH}", Lang.GetText("Hour") },
                { "{mm}", Lang.GetText("Minute") },
                { "{SS}", Lang.GetText("Second") }
            };

            var menu = new ContextMenu();
            foreach (var vp in patterns)
            {
                var mi = new MenuItem()
                {
                    Header = string.Format("{0}{1}", vp.Key.PadRight(8, ' '), vp.Value),
                    Tag = vp.Key
                };
                mi.Click += MenuVersionPattern_Click;
                menu.Items.Add(mi);
            }

            BtnVersionPatterns.ContextMenu = menu;
            menu.PlacementTarget = BtnVersionPatterns;
            menu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
        }

        private void MenuVersionPattern_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is MenuItem mi)
            {
                if (!TxbVersionPattern.IsReadOnly && mi.Tag is string vp)
                {
                    TxbVersionPattern.SelectedText = vp;
                }
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if(e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (Package != null)
            {
                Model.SaveTo(Package);

                DialogResult = true;
                Close();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
