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
using EmailClient.ViewModel;
using MimeKit;

namespace EmailClient.View
{
    /// <summary>
    /// Interaction logic for ReadEmail.xaml
    /// </summary>
    public partial class ReadEmail : Window
    {
        public ReadEmail(MimeMessage message)
        {
            InitializeComponent();
            DataContext = new ReadEmailViewModel(message);
        }
    }
}
