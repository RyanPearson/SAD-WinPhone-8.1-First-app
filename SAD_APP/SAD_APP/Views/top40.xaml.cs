using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using System.IO.IsolatedStorage;
using Newtonsoft.Json.Linq;
using SAD_APP.ServiceReference1;

// required for our music Client - ensure added in references folder

using System.Xml;

using System.Xml.Linq;

namespace SAD_APP.Views
{
    public partial class top40 : PhoneApplicationPage
    {
        List<SongDetails> contentList = new List<SongDetails>();
        public top40()
        {
            InitializeComponent();
        }
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)//, ref List<SongDetails> contentList)
        {
            try
            {
                Service1Client svc = new Service1Client();

                svc.GetTop40Completed += new EventHandler<GetTop40CompletedEventArgs>(svc_GetTop40Completed);

                svc.GetTop40Async();

            }catch
            {
                //Because the other parameters are not set by the user, the only error that can occur here is if the network has failed.
                MessageBox.Show("Network Error, please try again when the network has returned.");
            }
        }
        // executes when top 40 xml data has been downloaded

        // you can directly manipulate the xml file via XDocument xdox if you wish

        void svc_GetTop40Completed(object sender, GetTop40CompletedEventArgs e)
        {

            // parse downloaded xml into an instance of Xdocument called xdoc

            XDocument xdoc = XDocument.Parse(e.Result);

            // Create new list for songs in SongDetails class

            

            // For each top 40 'song' read in downloaded xml file add it to list

            // Use linq query to find each song in xml file

            // Use XElement method to parse xml to get to 'alt' and 'source' attributes of element 'img'



            foreach (XElement item in xdoc.Elements("query").Elements("results").Elements("img"))
            {

                // Create new song for each one in Top 40 (track name and cover art URL) and add to contentList list

                SongDetails content = new SongDetails();

                content.SongName = item.FirstAttribute.Value;

                content.ImageSource = item.FirstAttribute.NextAttribute.Value;

                content.SongArtist = item.NextNode.ToString().TrimStart("<a>".ToCharArray()).TrimEnd("</a>".ToCharArray());

                contentList.Add(content);

            }

            // add songs to ListBox for displaying from contentList list

            songList.ItemsSource = contentList.ToList();

        }
        // Helper class for adding song details to Listbox



        public class SongDetails
        {

            public string SongName { get; set; }

            public string ImageSource { get; set; }

            public string SongArtist { get; set; }

        }
        public List<SongDetails> PassContentList()
        {
            return (contentList);
        }

        private void Lyrics_Click(object sender, RoutedEventArgs e)
        {
            //Takes song name from the botton pressed
            var obj = sender as Button;
            var content = obj.Content;
            string SongName = content.ToString();
            List<SongDetails> contentList = PassContentList();
            //Searched for the song name in the list retrieved and returns the artist's name
            string SongArtist = "";
            foreach (SongDetails song in contentList)
            {
                if(song.SongName == SongName)
                {
                    SongArtist = song.SongArtist;
                    break;
                }
            }

            //Replaces all spaces in the song details, this is needed for the lyrics request
            SongName = SongName.Replace(" ", "_");
            SongArtist = SongArtist.Replace(" ", "_");

            //Encodes the song deatails to pass to the next page. This is because the characters are not gauranteed to be alphanumeric
            string encodedSongAtrist = Uri.EscapeDataString(SongArtist);
            string encodedSongName = Uri.EscapeDataString(SongName);
            NavigationService.Navigate(new Uri("/views/search.xaml?encodedSongName=" + encodedSongName + "&encodedSongArtist=" + encodedSongAtrist, UriKind.Relative));
        }
    }
}