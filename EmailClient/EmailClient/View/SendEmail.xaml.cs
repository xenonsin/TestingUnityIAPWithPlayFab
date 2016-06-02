using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
    /// Interaction logic for SendEmail.xaml
    /// </summary>
    public partial class SendEmail : Window
    {
        public enum SendType
        {
            Normal,
            Reply,
            Forward
        }
        public SendEmail(MimeMessage reply = null, SendType type = SendType.Normal)
        {
            InitializeComponent();
            SendEmailViewModel vm = new SendEmailViewModel(reply, type);
            DataContext = vm;
            if (vm.CloseAction == null)
                vm.CloseAction = new Action(() => this.Close());
        }
    }
}
