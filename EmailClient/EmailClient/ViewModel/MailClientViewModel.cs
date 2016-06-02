using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using EmailClient.Annotations;
using EmailClient.Commands;
using EmailClient.View;
using MailKit;
using MailKit.Net.Imap;
using MimeKit;

namespace EmailClient.ViewModel
{
    class MailClientViewModel : INotifyPropertyChanged
    {
        public ICommand CreateNewEmailCommand { get; }
        public ICommand SearchEmailCommand { get; }

        // Sorting Commands
        public ICommand SortByDateCommand { get; }
        public ICommand SortByFromCommand { get; }

        // Navigation Commands
        public ICommand DisplayTenCommand { get; }
        public ICommand DisplayTwentyCommand { get; }
        public ICommand NavigateToFirstcommand { get; }
        public ICommand NavigateToPrevCommand { get; }
        public ICommand NavigateToNextCommand { get; }
        public ICommand NavigateToLastCommand { get; }

        // Email Commands
        public ICommand ShowEmailWindowCommand { get; }
        public ICommand DeleteEmailCommand { get; }
        public ICommand RefreshEmailCommand { get; }
        

        // Observable Properties
        public  ObservableCollection<MimeMessage> Emails { get; set; } = new ObservableCollection<MimeMessage>();
        public  ObservableCollection<IMailFolder> Folders { get; set; } = new ObservableCollection<IMailFolder>(); 
        private MimeMessage selectedEmail;
        public MimeMessage SelectedMessage
        {
            get { return selectedEmail; }
            set
            {
                selectedEmail = value;
                OnPropertyChanged();
            }
        }

        private IMailFolder selectedFolder;

        public IMailFolder SelectedFolder
        {
            get { return selectedFolder;}
            set
            {
                selectedFolder = value;
                OnPropertyChanged();
            }
        }
        public MailClientViewModel()
        {
            CreateNewEmailCommand = new Command(x => CreateNewEmail());
            SearchEmailCommand = new Command(x => SearchEmail());

            ShowEmailWindowCommand = new Command(x => ShowEmailWindow());
            RefreshEmailCommand = new Command(x => RetrieveMail());
            RetrieveMail();
        }

        // retrieves mail
        private void RetrieveMail()
        {
            Emails.Clear();
            Folders.Clear();
            // connect to google 
            using (var client = new ImapClient())
            {
                client.Connect("imap.gmail.com", 993, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate("ece433tester","thisclassman");
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);
                for (int i = 0; i < inbox.Count; i++)
                {
                    var message = inbox.GetMessage(i);
                    Emails.Add(message);
                    Console.WriteLine("Subject: {0}", message.Subject);
                }
                Folders.Add(client.Inbox);

                if ((client.Capabilities & (ImapCapabilities.SpecialUse | ImapCapabilities.XList)) != 0)
                {
                    foreach (SpecialFolder folder in Enum.GetValues(typeof(SpecialFolder)))
                    {
                        var fold = client.GetFolder(folder);
                        if (fold != null)
                        {
                            fold.Open(FolderAccess.ReadOnly);
                            Folders.Add(client.GetFolder(folder));

                        }

                    }
                }
                if (Folders.Count > 0)
                    selectedFolder = Folders[0];

                client.Disconnect(true);
            }
        }

        // creates a popup and allows you to build and send an email
        private void CreateNewEmail()
        {
            // show popup dialog
            var sendEmailWindow = new SendEmail();
            sendEmailWindow.Show();
        }

        // searches the email list for a specific email
        private void SearchEmail()
        {
            
        }

        // shwo the email window
        private void ShowEmailWindow()
        {
            Console.WriteLine("You double clicked!");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
