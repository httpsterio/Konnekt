using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Windows.Networking.Proximity;
using Windows.Phone.PersonalInformation;
using Windows.Storage;
using Windows.Storage.Streams;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ConnectApp.Resources;
using UnicodeEncoding = System.Text.UnicodeEncoding;

namespace ConnectApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();       
        }

      

        private void SaveClicked(object sender, EventArgs e)
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
                using (var stream = storagefile.CreateFile("vcard.vcf"))
                {
                    using (var sw = new StreamWriter(stream))
                    {
                        sw.Write(sb);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something bad happened while saving the informaiton");
            }
        }

        private void ShareClicked(object sender, EventArgs e)
        {
            ProximityDevice device = ProximityDevice.GetDefault();

            // Make sure NFC is supported
            if (device != null)
            {
                var stream = IsolatedStorageFile.GetUserStoreForApplication().OpenFile("vcard.vcf", FileMode.Open);
                var sr = new StreamReader(stream);
                var vcard = sr.ReadToEnd();
                var dataWriter = new DataWriter() { UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8 };
                dataWriter.WriteString(vcard);
                device.PublishBinaryMessage("WindowsMime.text/vcard", dataWriter.DetachBuffer(), MesssageTransmitted);

            }
        }

        private void MesssageTransmitted(ProximityDevice sender, long messageid)
        {
            Dispatcher.BeginInvoke(() => MessageBox.Show("Vcard transfered succesfully"));
        }
    }
}