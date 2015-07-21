using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Threading;



namespace SongReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private MediaPlayer player = new MediaPlayer();

        private DispatcherTimer songTime = new DispatcherTimer();

        //used to tell if first song loaded
        private int firstTime = 0;

        private void loadFile(object sender, RoutedEventArgs e)
        {

            if (firstTime == 1)
            {
                //stops song to add new one
                cleanClose();
            }

            OpenFileDialog openDialog = new OpenFileDialog();
            //uncomment for a starting path
            //openDialog.InitialDirectory = "C:";
            
            //options for types of files allowed
            openDialog.Filter = "MP3 files (*.mp3)|*.mp3|WAV Files|*wav|All Files|*.*";


            

            if (openDialog.ShowDialog() == true)
            {

                //MediaOpened needed for TimeSpan
                player.MediaOpened += new EventHandler(player_MediaOpened);

                //shows songfile and opens in in the mediaplayer
                songTitle.Content = openDialog.SafeFileName;
                player.Open(new Uri(openDialog.FileName));
                
                //sets default items
                playStatus.Content = "Status: Play";

                if (firstTime == 0)
                {
                    volumeLabel.Content = "Volume: 50%";
                    volumeSlider.Value = .5;

                    //sets up timer to track song progress
                    songTime.Tick += songTime_Tick;
                    songTime.Interval = new TimeSpan(0, 0, 1);
                }
                
                
           
                //plays song then starts timer
                player.Play();
                songTime.Start();

                //marks first time loaded a song
                firstTime = 1;
                
                
            }
        }

        void songTime_Tick(object sender, EventArgs e)
        {
            //shows time where song is
            songStartTime.Content = TimeSpan.FromSeconds(player.Position.TotalSeconds).ToString(@"mm\:ss");

            //shows percent of song done on progress bar
            var percentDone = player.Position.TotalSeconds / player.NaturalDuration.TimeSpan.TotalSeconds * 100;
            songProgress.Value = percentDone;
        }
       
        

        private void player_MediaOpened(object sender, EventArgs e)
        {
            if (player.NaturalDuration.HasTimeSpan)
            {
                //gets song's duration 
               songEndTime.Content = TimeSpan.FromSeconds(player.NaturalDuration.TimeSpan.TotalSeconds).ToString(@"mm\:ss");
            }
        }

        private void Pause(object sender, RoutedEventArgs e)
        {
            player.Pause();
            playStatus.Content = "Status: Pause";
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            player.Stop();
            playStatus.Content = "Status: Stop";
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            cleanClose();
        }

        public void cleanClose()
        {
            player.Close();
            //set values back to zero
            playStatus.Content = "Status: Off";
            songTitle.Content = null;
            songProgress.Value = 0;
            songStartTime.Content = "00:00";
            songEndTime.Content = "00:00";
            //stops timer from crashing program
            songTime.Stop();
        }

        private void Play(object sender, RoutedEventArgs e)
        {
            player.Play();
            playStatus.Content = "Status: Play";
        }

        private void volumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Volume = volumeSlider.Value;
            volumeLabel.Content = "Volume: " + (Math.Round(volumeSlider.Value * 100, 0)) + "%";
        }

        
    }
}
