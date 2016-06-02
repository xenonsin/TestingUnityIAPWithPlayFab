using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using EmailClient.Annotations;
using EmailClient.Commands;
using EmailClient.View;
using MimeKit;

namespace EmailClient.ViewModel
{
    class ReadEmailViewModel : INotifyPropertyChanged
    {
        #region Properties
        private string fromField;
        public string FromField
        {
            get { return fromField; }
            set
            {
                if (!string.Equals(fromField, value))
                {
                    fromField = value;
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

        private string dateField;
        public string DateField
        {
            get { return dateField; }
            set
            {
                if (!string.Equals(dateField, value))
                {
                    dateField = value;
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

        public ICommand DownloadAttachCommand { get; }
        public ICommand ForwardCommand { get; }
        public ICommand ReplyCommand { get; }
        public ICommand DeleteCommand { get; }

        private MimeMessage currentMessage;

        public ReadEmailViewModel(MimeMessage message)
        {
            DownloadAttachCommand = new Command(x => DownloadAttach());
            ForwardCommand = new Command(x=> Forward());
            ReplyCommand = new Command(x => Reply());
            DeleteCommand = new Command(x => Delete());
            if (message != null)
            {
                currentMessage = message;
                FromField = message.From.ToString();
                CcField = message.Cc.ToString();
                SubjectField = message.Subject;
                DateField = message.Date.ToString();
                BodyField = message.TextBody;

                if (!message.Attachments.Any())
                    AttachField = "No Attachments.";

                foreach (var attachment in message.Attachments)
                {
                    string fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;
                    AttachField = AttachField + fileName + ", ";
                }
            }
           
        }

        private void DownloadAttach()
        {
            
        }

        private void Forward()
        {
            var sendEmailWindow = new SendEmail(currentMessage, SendEmail.SendType.Forward);
            sendEmailWindow.Show();
        }

        private void Reply()
        {
            var sendEmailWindow = new SendEmail(currentMessage, SendEmail.SendType.Reply);
            sendEmailWindow.Show();
        }

        private void Delete()
        {
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
