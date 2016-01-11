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
using AmeCreateProject.ViewModel;

namespace AmeCreateProject.Views
{
    /// <summary>
    /// Interaction logic for AmeProjectView.xaml
    /// </summary>
    public partial class AmeProjectView : UserControl
    {
        public AmeProjectView(AmeProjectViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
