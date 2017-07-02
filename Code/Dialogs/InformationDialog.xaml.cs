﻿using System;
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
    public partial class InformationDialog : Window
    {
        public static readonly DependencyProperty MessageProeprty = DependencyProperty.Register("Message", typeof(string), typeof(InformationDialog));

        public InformationDialog()
        {
            InitializeComponent();
        }

        public string Message
        {
            get => (string)GetValue(MessageProeprty);
            set => SetValue(MessageProeprty, value);
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
        }
    }
}
