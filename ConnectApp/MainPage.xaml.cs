using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Windows.Networking.Proximity;
using Windows.Phone.PersonalInformation;
using Windows.Storage;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ConnectApp.Resources;

namespace ConnectApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }


        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
        private async void SaveClicked(object sender, EventArgs e)
        {
            string fullName = FullNameTextBox.Text;
            if (string.IsNullOrWhiteSpace(fullName))
            {
                MessageBox.Show("Name can't be empty");
                return;
            }
            var firstName = fullName.Split(' ')[0];
            var lastName = fullName.Split(' ')[1];
            var sb = new StringBuilder();
            sb.AppendLine("BEGIN:VCARD");
            sb.AppendLine("VERSION:4.0");
            sb.AppendLine("N:" + firstName + ";" + lastName + ";;;");
            sb.AppendLine("FN:" + fullName);
            if (!string.IsNullOrWhiteSpace(OrganizationTextBox.Text))
            {
                sb.AppendLine("ORG:" + OrganizationTextBox.Text);
            }
            if (!string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                sb.AppendLine("EMAIL:" + EmailTextBox.Text);
            }
            if (!string.IsNullOrWhiteSpace(PhoneNumberTextBox.Text))
            {
                sb.AppendLine("TEL;CELL;VOICE:" + PhoneNumberTextBox.Text);
            }
            sb.AppendLine("END:VCARD");

            var storagefile = IsolatedStorageFile.GetUserStoreForApplication();
            if (storagefile.FileExists("vcard.vcf"))
            {
                storagefile.DeleteFile("vcard.vcf");
            }

            try
            {
                var stream = storagefile.CreateFile("vcard.vcf");
                var sw = new StreamWriter(stream);
                sw.Write(sb);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something bad happened while saving the informaiton");
            }
        }
    }
}