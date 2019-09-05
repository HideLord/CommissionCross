using Cross.Data;
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
    /// Interaction logic for TemplateButtonControl.xaml
    /// </summary>
    public partial class TemplateButtonControl : UserControl
    {
        private static readonly DependencyProperty templateProp =
            DependencyProperty.Register("TemplateProp", typeof(TemplateFormData), typeof(TemplateButtonControl));
        private static readonly DependencyProperty onClickCommand =
            DependencyProperty.Register("OnClick", typeof(ICommand), typeof(TemplateButtonControl));
        private static readonly DependencyProperty deleteCommand =
            DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(TemplateButtonControl));

        public TemplateFormData TemplateProp
        {
            get => (TemplateFormData)GetValue(templateProp);
            set => SetValue(templateProp, value);
        }
        public ICommand OnClick
        {
            get => (ICommand)GetValue(onClickCommand);
            set => SetValue(onClickCommand, value);
        }
        public ICommand DeleteCommand
        {
            get => (ICommand)GetValue(deleteCommand);
            set => SetValue(deleteCommand, value);
        }

        public TemplateButtonControl()
        {
            InitializeComponent();
        }

        private void OnClickHandler(object sender, RoutedEventArgs e)
        {
            OnClick.Execute(TemplateProp);
        }

        private void DeleteHandler(object sender, RoutedEventArgs e)
        {
            DeleteCommand.Execute(TemplateProp);
        }
    }
}
