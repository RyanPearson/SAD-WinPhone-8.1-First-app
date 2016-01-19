using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.IO.IsolatedStorage;

namespace SAD_APP.Views
{
    public partial class searchArtist : PhoneApplicationPage
    {
        public searchArtist()
        {
            InitializeComponent();
        }

        LyricData lyricInfo = new LyricData();

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Imports song details from top 40 page
            string encodedSongName = NavigationContext.QueryString["encodedSongName"];
            string encodedSongArtist = NavigationContext.QueryString["encodedSongArtist"];
            string SongName = Uri.UnescapeDataString(encodedSongName);
            string SongArtist = Uri.UnescapeDataString(encodedSongArtist);

            //Display song details with spaces instead of underscores
            tbSongArtist.Text = SongArtist.Replace("_", " ");
            tbSongName.Text = SongName.Replace("_", " ");

            WebClient lyricAPI = new WebClient();
            try
            {
                lyricAPI.DownloadStringCompleted += new DownloadStringCompletedEventHandler(lyricAPI_DownloadStringCompleted);

                lyricAPI.DownloadStringAsync(new Uri("http://lyrics.wikia.com/api.php?func=getSong&artist=" + SongArtist + "&song=" + SongName + "&fmt=realjson"));
            }
            catch
            {
                MessageBox.Show("Network Error, please try again when the network has returned.");
            }

        }
        void lyricAPI_DownloadStringCompleted(object senders, DownloadStringCompletedEventArgs e)
        {
            //Stores all information from the JSON object returned as a LyricData datatype.
            //checks if the json object is valid
            if (e.Error == null && e.Result.StartsWith("{"))
            lyricInfo = JsonConvert.DeserializeObject<LyricData>(e.Result);
            try
            {
                wbDisplayLyrics.Navigate(new Uri(lyricInfo.url, UriKind.Absolute));
            }catch
            { MessageBox.Show("Network Error, please try again when the network has returned."); }
        }

        public class LyricData
        {
           public string artist { get; set; }

           public string song { get; set; }

           public string lyrics { get; set; }

           public string url { get; set; }

           public int page_namespace { get; set; }

           public int page_id { get; set; }

           public int isOnTakeDownList { get; set; }
        }
       
    }
}