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
using MailKit.Search;
using MimeKit;

namespace EmailClient.ViewModel
{
    class MailClientViewModel : INotifyPropertyChanged
    {
        public ICommand CreateNewEmailCommand { get; }
        public ICommand SearchEmailCommand { get; }

        // Sorting Commands
        public ICommand SortByDateDownCommand { get; }
        public ICommand SortByDateUpCommand { get; }

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
        public ICommand ShowFolderCommand { get; }

        // Observable Properties
        public  ObservableCollection<MimeMessage> Emails { get; set; } = new ObservableCollection<MimeMessage>();
        public  ObservableCollection<IMailFolder> Folders { get; set; } = new ObservableCollection<IMailFolder>();
        private List<SpecialFolder> folderTypes { get; set; } = new List<SpecialFolder>(); 
        private int selectedEmailIndex;
        public int SelectedEmailIndex
        {
            get { return selectedEmailIndex; }
            set
            {
                if (value >= 0)
                {
                    selectedEmailIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        private int selectedFolderIndex = 0;

        public int SelectedFolderIndex
        {
            get { return selectedFolderIndex; }
            set
            {
                if (selectedFolderIndex != value && value >= 0)
                {
                    selectedFolderIndex = value;
                    OnPropertyChanged();
                    ShowFolder();
                }
            }
        }
        public MailClientViewModel()
        {
            CreateNewEmailCommand = new Command(x => CreateNewEmail());
            DeleteEmailCommand = new Command(x => DeleteEmail());
            ShowEmailWindowCommand = new Command(x => ShowEmailWindow());
            RefreshEmailCommand = new Command(x => RetrieveMail());

            SortByDateDownCommand = new Command(x => SortByDateDown());
            SortByDateUpCommand = new Command(x => SortByDateUp());
            RetrieveMail();
        }

        private async void SortByDateDown()
        {
            var folder = Folders[SelectedFolderIndex];
            if (folder == null) return;
            Emails.Clear();
            using (var client = new ImapClient())
            {
                await client.ConnectAsync("imap.gmail.com", 993, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate("ece433tester", "thisclassman");
                IMailFolder fold;
                if (folder.Name == "INBOX")
                {
                    fold = client.Inbox;
                    await fold.OpenAsync(FolderAccess.ReadOnly);
                    for (int i = 0; i > fold.Count; i++)
                    {
                        try
                        {
                            var email = fold.GetMessage(i);
                            Emails.Add(email);
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                }
                else
                {

                    fold = client.GetFolder(folderTypes[SelectedFolderIndex - 1]);
                    await fold.OpenAsync(FolderAccess.ReadOnly);
                    for (int i = 0; i > fold.Count; i++)
                    {
                        try
                        {
                            var email = fold.GetMessage(i);
                            Emails.Add(email);
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                }
                client.Disconnect(true);
            }
        }

        private async void SortByDateUp()
        {
            var folder = Folders[SelectedFolderIndex];
            if (folder == null) return;
            Emails.Clear();
            using (var client = new ImapClient())
            {
                await client.ConnectAsync("imap.gmail.com", 993, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate("ece433tester", "thisclassman");
                IMailFolder fold;
                if (folder.Name == "INBOX")
                {
                    fold = client.Inbox;
                    await fold.OpenAsync(FolderAccess.ReadOnly);
                    for (int i = fold.Count -1; i >= 0; i--)
                    {
                        try
                        {
                            var email = fold.GetMessage(i);
                            Emails.Add(email);
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                }
                else
                {

                    fold = client.GetFolder(folderTypes[SelectedFolderIndex - 1]);
                    await fold.OpenAsync(FolderAccess.ReadOnly);
                    for (int i = fold.Count - 1; i >= 0; i--)
                    {
                        try
                        {
                            var email = fold.GetMessage(i);
                            Emails.Add(email);
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                }
                client.Disconnect(true);
            }
            
        }

        // retrieves mail
        private async void RetrieveMail()
        {
            Folders.Clear();
            Emails.Clear();
            folderTypes.Clear();
            // connect to google 
            using (var client = new ImapClient())
            {
                await client.ConnectAsync("imap.gmail.com", 993, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate("ece433tester","thisclassman");
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);
                Folders.Add(client.Inbox);
                if ((client.Capabilities & (ImapCapabilities.SpecialUse | ImapCapabilities.XList)) != 0)
                {
                    foreach (SpecialFolder folder in Enum.GetValues(typeof(SpecialFolder)))
                    {
                        var fold = client.GetFolder(folder);
                        if (fold != null)
                        {
                            fold.Open(FolderAccess.ReadOnly);
                            folderTypes.Add(folder);
                            Folders.Add(client.GetFolder(folder));
                        }

                    }
                }
                
                ShowFolder();

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

        private async void DeleteEmail()
        {
            var folder = Folders[SelectedFolderIndex];
            if (folder == null) return;
            using (var client = new ImapClient())
            {
                await client.ConnectAsync("imap.gmail.com", 993, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate("ece433tester", "thisclassman");
                IMailFolder fold;
                if (folder.Name == "INBOX")
                {
                    fold = client.Inbox;
                    await fold.OpenAsync(FolderAccess.ReadWrite);
                    fold.AddFlags(SelectedEmailIndex, MessageFlags.Deleted, true);
                }
                else if (folder.Name == "Trash")
                {
                    fold = client.GetFolder(SpecialFolder.Trash);
                    fold.AddFlags(SelectedEmailIndex, MessageFlags.Deleted, true);
                    fold.Expunge();
                }
                else
                {

                    fold = client.GetFolder(folderTypes[SelectedFolderIndex - 1]);
                    await fold.OpenAsync(FolderAccess.ReadWrite);
                    fold.AddFlags(SelectedEmailIndex, MessageFlags.Deleted, true);
                }
                RetrieveMail();
                client.Disconnect(true);
            }
        }

        // shwo the email window
        private void ShowEmailWindow()
        {
            var readEmailWindow = new ReadEmail (Emails[SelectedEmailIndex]);
            readEmailWindow.Show();
        }

        private async void ShowFolder()
        {            
            var folder = Folders[SelectedFolderIndex];
            if (folder == null) return;
            Emails.Clear();
            using (var client = new ImapClient())
            {
                await client.ConnectAsync("imap.gmail.com", 993, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate("ece433tester", "thisclassman");
                IMailFolder fold;
                if (folder.Name == "INBOX")
                {
                    fold = client.Inbox;
                    await fold.OpenAsync(FolderAccess.ReadOnly);
                    for (int i = 0; i < fold.Count; i++)
                    {
                        try
                        {
                            var email = fold.GetMessage(i);
                            Emails.Add(email);
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                }
                else
                {
                    if ((client.Capabilities & (ImapCapabilities.SpecialUse | ImapCapabilities.XList)) != 0)
                    {
                        fold = client.GetFolder(folderTypes[SelectedFolderIndex-1]);
                        await fold.OpenAsync(FolderAccess.ReadOnly);
                        for (int i = 0; i < fold.Count; i++)
                        {
                            try
                            {
                                var email = fold.GetMessage(i);
                                Emails.Add(email);
                            }
                            catch (Exception)
                            {

                                throw;
                            }
                        }
                    }
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
