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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_Cross.UserControls
{
    /// <summary>
    /// Interaction logic for EditorControl.xaml
    /// </summary>
    public partial class EditorControl : UserControl
    {
        private const double delta = 5;

        private static readonly DependencyProperty headerProp =
            DependencyProperty.Register("Header", typeof(string), typeof(EditorControl));

        private static readonly DependencyProperty bindingProp =
            DependencyProperty.Register("EditedVal", typeof(double), typeof(EditorControl));

        public string Header
        {
            get=> (string)GetValue(headerProp);
            set=> SetValue(headerProp,value);
        }

        public double EditedVal
        {
            get => (double)GetValue(bindingProp);
            set => SetValue(bindingProp, value);
        }

        public EditorControl()
        {
            InitializeComponent();
        }

        private void Decrease(object sender, RoutedEventArgs e)
        {
            EditedVal -= delta;
        }
        private void Increase(object sender, RoutedEventArgs e)
        {
            EditedVal += delta;
        }
    }
}
