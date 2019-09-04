using Autofac;
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
using WPF_Cross.ViewModels;

namespace WPF_Cross.Views
{
    /// <summary>
    /// Interaction logic for CreateSaveView.xaml
    /// </summary>
    public partial class CreateSaveView : Page
    {
        public CreateSaveView()
        {
            InitializeComponent();
            this.DataContext = Bootstrapper.Container.Resolve<CreateSaveViewModel>();
        }
    }
}
