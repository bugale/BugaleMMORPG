using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace BugaleMMORPGWPFClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Caches bitmaps of monster and player
        private BitmapImage _player_image = null;
        private BitmapImage _monster_image = null;

        // A queue for command from the GUI that will be sent to the server (through Client.dll)
        private ConcurrentQueue<String> _command_queue = new ConcurrentQueue<String>();

        // The interface with the server (from Client.dll), and the thread in which it is being run
        private BugaleMMORPG.Client _client = null;
        private Thread _client_thread = null;

        // An array of Image controls for monsters/players
        private System.Windows.Controls.Image[] _map = new System.Windows.Controls.Image[30];

        // An array of label controls for HP of monsters/players
        private System.Windows.Controls.Label[] _hp = new System.Windows.Controls.Label[30];

        private static BitmapImage _process_bitmap(Bitmap image)
        {
            BitmapImage result = new BitmapImage();

            using (MemoryStream memory = new MemoryStream())
            {
                image.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                result.BeginInit();
                result.StreamSource = memory;
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.EndInit();
            }

            return result;
        }

        public MainWindow()
        {
            InitializeComponent();

            // Load images of player and monster
            this._player_image = MainWindow._process_bitmap(BugaleMMORPGWPFClient.Properties.Resources.player);
            this._monster_image = MainWindow._process_bitmap(BugaleMMORPGWPFClient.Properties.Resources.monster);

            // Create the array of images and labels for the map and HP
            for (Int32 i = 0; i < this._map.Length; i++)
            {
                this._map[i] = new System.Windows.Controls.Image();
                this._hp[i] = new System.Windows.Controls.Label();
                MapGrid.Children.Add(this._map[i]);
                HPGrid.Children.Add(this._hp[i]);

                this._map[i].HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                this._map[i].Height = MapGrid.Height;
                this._map[i].Width = MapGrid.Width / this._map.Length;
                this._map[i].Margin = new Thickness(this._map[i].Width * i, 0, 0, 0);
                this._map[i].Stretch = Stretch.Fill;

                this._hp[i].HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                this._hp[i].Margin = new Thickness(this._map[i].Width * i - 7, 0, 0, 0);
            }
            this._clear_map();

            // Start the interface with the server (from Client.dll)
            this._client = new BugaleMMORPG.Client(this._dispatch_redraw, this._command_queue);
            this._client_thread = new Thread(new ThreadStart(this._client.Run));
            this._client_thread.Start();
        }

        private void _clear_map()
        {
            for (Int32 i = 0; i < this._map.Length; i++)
            {
                this._map[i].Source = null;
                this._hp[i].Content = "";
            }
        }

        private void _dispatch_redraw(String state)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() => this._redraw_game_state(state)));
        }

        private void _redraw_game_state(String state)
        {
            IEnumerable<String> monsters =
                   from m in state.Split(';')
                   where m.StartsWith("Monsters")
                   select m;
            IEnumerable<String> players =
                from p in state.Split(';')
                where p.StartsWith("Players")
                select p;
            String current_player = state.Split(';').First();

            // Initialize map
            this._clear_map();

            // Put monsters
            foreach (String monster in monsters)
            {
                this._hp[Int32.Parse(monster.Split(',')[2])].Content = monster.Split(',')[3];
                this._map[Int32.Parse(monster.Split(',')[2])].Source = this._monster_image;
            }

            // Put players
            foreach (String player in players)
            {
                this._hp[Int32.Parse(player.Split(',')[2])].Content = player.Split(',')[3];
                this._map[Int32.Parse(player.Split(',')[2])].Source = this._player_image;
            }

            // Print player stats
            LevelLabel.Content = current_player.Split(',')[3];
            HPLabel.Content = current_player.Split(',')[2];
            WeaponAttackLabel.Content = current_player.Split(',')[1];
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this._client_thread.Abort();
        }

        private void Window_KeyUp_1(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Left)
                this._command_queue.Enqueue("L");
            else if (e.Key == System.Windows.Input.Key.Right)
                this._command_queue.Enqueue("R");
            else if (e.Key == System.Windows.Input.Key.Enter)
                this._command_queue.Enqueue("A");
        }
    }
}
