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

        // Observable Properties
        public  ObservableCollection<MimeMessage> Emails { get; set; } = new ObservableCollection<MimeMessage>();
        
        public MailClientViewModel()
        {
            CreateNewEmailCommand = new Command(x => CreateNewEmail());
            SearchEmailCommand = new Command(x => SearchEmail());

            RetrieveMail();
        }

        // retrieves mail
        public void RetrieveMail()
        {
            // connect to google 
            using (var client = new ImapClient())
            {
                client.Connect("imap.gmail.com", 993, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate("ece433tester","thisclassman");
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);
                Console.WriteLine("Total messages: {0}", inbox.Count);
                Console.WriteLine("Recent messages: {0}", inbox.Recent);

                for (int i = 0; i < inbox.Count; i++)
                {
                    var message = inbox.GetMessage(i);
                    Emails.Add(message);
                    Console.WriteLine("Subject: {0}", message.Subject);
                }

                client.Disconnect(true);
            }
        }

        // creates a popup and allows you to build and send an email
        private void CreateNewEmail()
        {
            // show popup dialog
        }

        // searches the email list for a specific email
        private void SearchEmail()
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
