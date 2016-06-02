using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using EmailClient.Annotations;
using EmailClient.Commands;
using MailKit.Net.Smtp;
using MimeKit;
using MessageBox = System.Windows.MessageBox;


namespace EmailClient.ViewModel
{
    class SendEmailViewModel : INotifyPropertyChanged
    {
    #region Properties
        private string toField;
        public string ToField
        {
            get { return toField;}
            set
            {
                if (!string.Equals(toField, value))
                {
                    toField = value;
                    OnPropertyChanged();
                }               
            }
        }

        private string ccField;
        public string CcField
        {
            get { return ccField; }
            set
            {
                if (!string.Equals(ccField, value))
                {
                    ccField = value;
                    OnPropertyChanged();
                }
            }
        }

        private string bccField;
        public string BccField
        {
            get { return bccField; }
            set
            {
                if (!string.Equals(bccField, value))
                {
                    bccField = value;
                    OnPropertyChanged();
                }
            }
        }

        private string subjectField;
        public string SubjectField
        {
            get { return subjectField; }
            set
            {
                if (!string.Equals(subjectField, value))
                {
                    subjectField = value;
                    OnPropertyChanged();
                }
            }
        }

        private string attachField;
        public string AttachField
        {
            get { return attachField; }
            set
            {
                if (!string.Equals(attachField, value))
                {
                    attachField = value;
                    OnPropertyChanged();
                }
            }
        }

        private string bodyField;
        public string BodyField
        {
            get { return bodyField; }
            set
            {
                if (!string.Equals(bodyField, value))
                {
                    bodyField = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
        public ICommand AttachCommand { get; }
        public ICommand SendCommand { get; }
        public Action CloseAction { get; set; }

        public SendEmailViewModel()
        {
            AttachCommand = new Command(x => Attach());
            SendCommand = new Command(x => Send());
        }

        // attach a file unto the email
        private void Attach()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = @"C:\";
            dialog.Title = "Attach File...";
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.DefaultExt = "txt";
            dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            dialog.FilterIndex = 2;
            dialog.RestoreDirectory = true;

            dialog.ReadOnlyChecked = true;
            dialog.ShowReadOnly = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                AttachField = dialog.FileName;
            }
        }
    

        // send the email
        private void Send()
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("Keno San Pablo", "ece433tester@gmail.com"));

            // parse to addresses
            if (!string.IsNullOrEmpty(toField))
            {
                string[] toAddresses = toField.Split(',');
                foreach (var address in toAddresses)
                {
                    if (!address.Contains('@')) continue;
                    string name = address.Split('@')[0];
                    message.To.Add(new MailboxAddress(name, address));
                }
            }
            // parse cc addresses
            if (!string.IsNullOrEmpty(ccField))
            {
                string[] ccAddresses = ccField.Split(',');
                foreach (var address in ccAddresses)
                {
                    if (!address.Contains('@')) continue;
                    string name = address.Split('@')[0];
                    message.Cc.Add(new MailboxAddress(name, address));
                }
            }

            // parse bcc addresses
            if (!string.IsNullOrEmpty(bccField))
            {
                string[] bccAddresses = bccField.Split(',');
                foreach (var address in bccAddresses)
                {
                    if (!address.Contains('@')) continue;
                    string name = address.Split('@')[0];
                    message.Bcc.Add(new MailboxAddress(name, address));
                }
            }

            message.Subject = subjectField;

            BodyBuilder builder = new BodyBuilder();
            builder.TextBody = bodyField;
            if (!string.IsNullOrEmpty(attachField))
            {
                try
                {
                    builder.Attachments.Add(attachField);
                }
                catch (Exception)
                {
                    
                    throw;
                }
            }
            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate("ece433tester", "thisclassman");

                try
                {
                    client.Send(message);
                    // Configure the message box to be displayed
                    string messageBoxText = "Message Sent!";
                    string caption = "Success";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Exclamation;
                    MessageBox.Show(messageBoxText, caption, button, icon);
                    CloseAction();
                }
                catch (Exception)
                {
                    
                    throw;
                }
                client.Disconnect(true);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
