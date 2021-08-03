using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using WMPLib;
using TagLib;
using System.Runtime.InteropServices;
using System.Diagnostics;
using NAudio;
using NAudio.Wave;

namespace PlayerUI
{
    public partial class PanelSideMenu : Form
    {
        public PanelSideMenu()
        {
            InitializeComponent();
            customizeDesign();
            trackBar1.Hide();
            trackBar.Hide();
        }

        private void customizeDesign()
        {
            panelMediaSubMenu.Visible = false;
            panelPlaylistSubMenu.Visible = false;
            panelToolsSubMenu.Visible = false;
        }

        private void hideSubMenu()
        {
            if (panelMediaSubMenu.Visible == true)
            {
                panelMediaSubMenu.Visible = false;
            }
            if (panelPlaylistSubMenu.Visible == true)
            {
                panelPlaylistSubMenu.Visible = false;
            }
            if (panelToolsSubMenu.Visible == true)
            {
                panelToolsSubMenu.Visible = false;
            }
        }

        private void showSubMenu(Panel subMenu)
        {
            if (subMenu.Visible == false)
            {
                hideSubMenu();
                subMenu.Visible = true;
            }
            else
            {
                subMenu.Visible = false;
            }
        }

        private void btnMedia_Click(object sender, EventArgs e)
        {
            showSubMenu(panelMediaSubMenu);
        }
        #region MediaSubMenu

        string withOutExt;
        string destination;
        double dur;

        List<string> musicNames = new List<string>();
        private void button2_Click(object sender, EventArgs e)//done open Musics multiselect = true
        {
            OpenFileDialog opnFile = new OpenFileDialog();

            opnFile.Filter = "mp3 files (*.mp3)|*.mp3|All files (*.*)|*.*";

            opnFile.Multiselect = true;

            if (opnFile.ShowDialog() == DialogResult.OK)
            {

                var address = opnFile.FileNames;

                for (int i = 0; i < opnFile.FileNames.Length; i++)
                {
                    var ext = Path.GetExtension(address[i]);

                    withOutExt = Path.GetFileNameWithoutExtension(address[i]);

                    listMusic.Items.Add(withOutExt.ToUpper());

                    musicNames.Add(withOutExt.ToUpper());

                    if (Path.GetExtension(ext) == ".mp3" || Path.GetExtension(ext)==".wav")
                    {
                        #region SavingInDB გავაუქმე :|
                        //MyConnectionDataContext db = new MyConnectionDataContext();

                        //db.PulseBeatsMusicDirectories.InsertOnSubmit(new PulseBeatsMusicDirectory
                        //{
                        //    Ext = ext,

                        //    MusicName = withOutExt,

                        //    PathFile = address[i],
                        //});
                        //db.SubmitChanges();
                        #endregion //გავაუქმე! :|
                        //creating folder on desktop
                        string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                        var folder = Path.Combine(path, "myPlaylist");

                        Directory.CreateDirectory(folder);

                        destination = path + "\\myPlaylist\\" + withOutExt + ext;

                        listMusic.SetSelected(0, true);

                        #region ვშლი ფაილს თუ უკვე არსებობს

                        string source2 = destination;

                        FileInfo ff = new FileInfo(source2);

                        ff.Delete();


                        #endregion

                        #region მომაქ ფაილი საწყისი პოზიციიდან და ვაგდებ myPlaylist-ში დესკტოპზე
                        try
                        {
                            string source = address[i];

                            FileInfo music = new FileInfo(source);

                            music.CopyTo(destination);
                        }catch(Exception ex)
                        {
                            MessageBox.Show(this, ex.Message);
                        }
                        

                        #endregion
                    }
                }
            }
            hideSubMenu();
        }
        private void button3_Click(object sender, EventArgs e)//done  open folder!
        {
            FolderBrowserDialog opnFolder = new FolderBrowserDialog();

            List<string> playlistFromFolder = new List<string>();

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            var folder = Path.Combine(path, "myPlaylist");

            Directory.CreateDirectory(folder);

            if (opnFolder.ShowDialog() == DialogResult.OK)
            {
                var address = opnFolder.SelectedPath;
                string[] playlist = Directory.GetFiles(address);
                for (int i = 0; i < playlist.Length; i++)
                {
                    if (playlist[i].Contains(".mp3"))
                    {
                        playlistFromFolder.Add(playlist[i].Trim(address.ToCharArray()));
                    }
                }
                for (int v = 0; v < playlistFromFolder.Count; v++)
                {
                    withOutExt = Path.GetFileNameWithoutExtension(playlistFromFolder[v]);
                    listMusic.Items.Add(withOutExt);
                    #region deletingIfExists
                    destination = path + "\\myPlaylist\\" + playlistFromFolder[v];
                    string source2 = destination;
                    FileInfo ff = new FileInfo(source2);
                    ff.Delete();
                    #endregion

                    #region CopyIfDoesntExist
                    string source = address + "\\" + playlistFromFolder[v];
                    FileInfo music = new FileInfo(source);
                    music.CopyTo(destination);
                    #endregion
                }
            }
            hideSubMenu();
        }

        [DllImport("winmm.dll")]
        static extern Int32 mciSendString(String command, StringBuilder buffer, Int32 bufferSize, IntPtr hwndCallback);
        bool isCDopened;
        private void closeCD()
        {
            //Closes the Cd Tray            
            mciSendString("set CDAudio door closed", null, 0, IntPtr.Zero);
        }
        private void openCD()
        {
            //Opens the Cd Tray of your cd Drive            
            mciSendString("set CDAudio door open", null, 0, IntPtr.Zero);
        }

        private void button4_Click(object sender, EventArgs e)//OpenDisk აღებს კომპიუტერის CD/DVD ჩასადებს!
        {
            if (isCDopened == false)
            {
                openCD();
                MessageBox.Show("ჩადეთ სასურველი CD :) და დააჭირეთ ისევ Media>OpenDisk-ს დასახურად");
                isCDopened = true;
            }
            else
            {
                closeCD();
            }

            hideSubMenu();
        } //done

        private void button5_Click(object sender, EventArgs e)//recentMedia button-click მოძებნის დესკტოპზე myPlaylist-დან!
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                var item = Directory.GetFiles(path + "\\myPlaylist\\");

                string forCut = path + "\\myPlaylist\\";

                for (int i = 0; i < item.Length; i++)
                {
                    string song = item[i].TrimStart(forCut.ToCharArray());
                    listMusic.Items.Add(song.TrimEnd(".mp3".ToCharArray()));
                }

                listMusic.SetSelected(0, true);

                //axWindowsMediaPlayer1.URL = path + "\\myPlaylist\\" + listMusic.SelectedItem + ".mp3";
                //axWindowsMediaPlayer1.Ctlcontrols.pause();
            }
            catch (Exception error)
            {
                MessageBox.Show("ბოლოს გამოყენებული მუსიკა ვერ მოიძებნა რადგან წაშლილია myPlaylist საქაღალდე დესკტოპიდან :)", error.Message);
            }

            hideSubMenu();
        }//done
        #endregion
        private void btnPlaylist_Click(object sender, EventArgs e)
        {
            showSubMenu(panelPlaylistSubMenu);
        }
        #region PlaylistSubMenu
        private void button9_Click(object sender, EventArgs e) //New Playlist აჩერებს პლეიერს, შლის ახლანდელ პლეილისტს და გვიგდებს ასარჩევად ახალ ფოლდერს.
        {

            axWindowsMediaPlayer1.Ctlcontrols.stop();
            listMusic.Items.Clear();

            FolderBrowserDialog opnFolder = new FolderBrowserDialog();

            List<string> playlistFromFolder = new List<string>();

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            var folder = Path.Combine(path, "myPlaylist");

            Directory.CreateDirectory(folder);

            if (opnFolder.ShowDialog() == DialogResult.OK)
            {
                var address = opnFolder.SelectedPath;
                string[] playlist = Directory.GetFiles(address);
                for (int i = 0; i < playlist.Length; i++)
                {
                    if (playlist[i].Contains(".mp3"))
                    {
                        playlistFromFolder.Add(playlist[i].Trim(address.ToCharArray()));
                    }
                }
                for (int v = 0; v < playlistFromFolder.Count; v++)
                {
                    withOutExt = Path.GetFileNameWithoutExtension(playlistFromFolder[v]);
                    listMusic.Items.Add(withOutExt);
                    #region deletingIfExists
                    destination = path + "\\myPlaylist\\" + playlistFromFolder[v];
                    string source2 = destination;
                    FileInfo ff = new FileInfo(source2);
                    ff.Delete();
                    #endregion

                    #region CopyIfDoesntExist
                    string source = address + "\\" + playlistFromFolder[v];
                    FileInfo music = new FileInfo(source);
                    music.CopyTo(destination);
                    #endregion
                }

            }



            hideSubMenu();
        } //done!

        private void button8_Click(object sender, EventArgs e)//დასაწერია Manage Playlist!!!!!!!!
        {
            //..
            //Your Code
            //..
            hideSubMenu();
        }

        private void button7_Click(object sender, EventArgs e)//Import Playlist იგივეა რაც Media>Open Folder ფილტრავს ფაილში mp3-ებს და გამოაქვს listMusic-ში.
        {
            FolderBrowserDialog opnFolder = new FolderBrowserDialog();

            List<string> playlistFromFolder = new List<string>();

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            var folder = Path.Combine(path, "myPlaylist");

            Directory.CreateDirectory(folder);

            if (opnFolder.ShowDialog() == DialogResult.OK)
            {
                var address = opnFolder.SelectedPath;
                string[] playlist = Directory.GetFiles(address);
                for (int i = 0; i < playlist.Length; i++)
                {
                    if (playlist[i].Contains(".mp3"))
                    {
                        playlistFromFolder.Add(playlist[i].Trim(address.ToCharArray()));
                    }
                }
                for (int v = 0; v < playlistFromFolder.Count; v++)
                {
                    withOutExt = Path.GetFileNameWithoutExtension(playlistFromFolder[v]);
                    listMusic.Items.Add(withOutExt);
                    #region deletingIfExists
                    destination = path + "\\myPlaylist\\" + playlistFromFolder[v];
                    string source2 = destination;
                    FileInfo ff = new FileInfo(source2);
                    ff.Delete();
                    #endregion

                    #region CopyIfDoesntExist
                    string source = address + "\\" + playlistFromFolder[v];
                    FileInfo music = new FileInfo(source);
                    music.CopyTo(destination);
                    #endregion
                }

            }
            hideSubMenu();
        }//done!

        private void button6_Click(object sender, EventArgs e)//გააქვს ექსპორტზე არჩეული playlist Desktop\Playlist-ში
        {

            string folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\ExportedPlaylist\\";
            string desk = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (Directory.Exists(folder) == false)
            {
                Directory.CreateDirectory(folder);
                for (int i = 0; i < listMusic.Items.Count; i++)
                {
                    string source = desk + "\\myPlaylist\\" + listMusic.Items[i] + ".mp3";
                    FileInfo music = new FileInfo(source);
                    string desttt = folder + listMusic.Items[i] + ".mp3";
                    music.CopyTo(desttt);
                }


                ProcessStartInfo startInfo = new ProcessStartInfo();
                {
                    startInfo.Arguments = folder;
                    startInfo.FileName = "explorer.exe";
                }

                Process.Start(startInfo);

            }
            else if (Directory.Exists(folder))
            {
                string[] source2 = Directory.GetFiles(folder);
                foreach (var item in source2)
                {
                    FileInfo fff = new FileInfo(item);
                    fff.Delete();
                }

                for (int i = 0; i < listMusic.Items.Count; i++)
                {
                    string source = desk + "\\myPlaylist\\" + listMusic.Items[i] + ".mp3";
                    FileInfo music = new FileInfo(source);
                    string desttt = folder + listMusic.Items[i] + ".mp3";
                    music.CopyTo(desttt);
                }

                ProcessStartInfo startInfo = new ProcessStartInfo();
                {
                    startInfo.Arguments = folder;
                    startInfo.FileName = "explorer.exe";
                }

                Process.Start(startInfo);
            }
            else
            {
                MessageBox.Show(string.Format("{0} ექსპორტი ვერ მოხერხდა!", folder));
            }

            hideSubMenu();
        }//done!
        #endregion
        public void btnEqualizer_Click(object sender, EventArgs e)
        {
            openChildForm(new Form2());
            //..
            //Your Code
            //..
            hideSubMenu();
        }

        private void btnTools_Click(object sender, EventArgs e)
        {
            showSubMenu(panelToolsSubMenu);
        }
        #region ToolsSubMenu
        private void button15_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "mp3 File (*.mp3)|*.mp3";
            if (open.ShowDialog() != DialogResult.OK) return;

            string output = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + $"{open.SafeFileName}" + "-" + "converted.wav";
            Mp3ToWav(open.FileName, output);

            MessageBox.Show(this, $"default output Path: {output}","Conversion was successful!");

            hideSubMenu();
        }

        //mp3 converter to wav file
        private void Mp3ToWav(string mp3File, string output)
        {
            using (Mp3FileReader reader = new Mp3FileReader(mp3File))
            {
                using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(reader))
                {
                    WaveFileWriter.CreateWaveFile(output, pcmStream);
                }
            }
        }


        private void button14_Click(object sender, EventArgs e)
        {
            //..
            //Your Code
            //..
            hideSubMenu();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            //..
            //Your Code
            //..
            hideSubMenu();
        }

        #endregion

        private void OpenBrowser()//მომხმარებელს გაუხსნის google chrome-ს ბრაუზერს Help-ღილაკზე დაჭერისას!
        {
            bool processStarted = false;

            Process[] processes = Process.GetProcesses();

            foreach (var item in processes)
            {
                if (item.MainWindowTitle.Equals("Google - Google Chrome", StringComparison.OrdinalIgnoreCase))
                {
                    processStarted = true;
                    break;
                }
            }

            if (!processStarted)
            {
                Process p = new Process();
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "chrome.exe";
                p.StartInfo = info;
                info.Arguments = "https://www.PulseBeats.Com/Help";
                p.Start();
            }
        } //done!
        private void btnHelp_Click(object sender, EventArgs e)// მომხმარებელს გაუხსნის googlechrome-ში Help ვებსაიტს!
        {

            OpenBrowser();

            hideSubMenu();
        } //done!

        private Form activeForm = null;
        private void openChildForm(Form childForm)
        {
            if (activeForm != null)
                activeForm.Close();
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panelChildForm.Controls.Add(childForm);
            panelChildForm.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        private void testPanel_Paint(object sender, PaintEventArgs e)
        {
            //nothing...
        }

        #region გლობალური ცვლადები
        bool isPlaying = false;
        bool resign = false;
        int intervalTime;
        int currentTime;
        int duration;
        #endregion
        bool isFromPlaylist;
        int ind = 0;
        private void timer3Calc() //ეს არის timer3-ის ინტერვალის კალკულატორი ტრეკის ხანგრძლივობიდან გამომდინარე
        {
            if (ind == droppedMusicName.Count)
            {
                ind = 0;
            }
            //var tFile = TagLib.File.Create(destination);
            string fileDir;
            if (isFromPlaylist)
            {
                fileDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\myPlaylist\\" + droppedMusicName[ind] + ".mp3";
            }
            else if (listMusic.SelectedItem != null)
            {
                fileDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\myPlaylist\\" + listMusic.SelectedItem + ".mp3";
            }
            else if (!isFromPlaylist)
            {
                fileDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\myPlaylist\\" + listMusic.SelectedItem + ".wav";
            }
            else
            {
                fileDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\myPlaylist\\" + listMusic.SelectedItem + ".mp3";
            }
            var tFile = TagLib.File.Create(fileDir);
            TimeSpan gg = tFile.Properties.Duration;
            dur = gg.TotalSeconds;
            double divide = dur / trackBar1.Maximum;
            double multiply = divide * 1000;
            intervalTime = Convert.ToInt32(multiply);
            trackBar1.TickStyle = TickStyle.None;
            trackBar1.TickFrequency = 100;
            timer3.Interval = intervalTime;
            ind++;
        } //done

        private void btnPlay_Click(object sender, EventArgs e)//მთავარი PLAY ღილაკის ფუნქციონალი.
        {
            try
            {
                timer1.Interval = 1000; //1 second! ანუ 1000ms.
                timer2.Interval = 1000;
                timer3Calc();

                if (resign == false)
                {
                    axWindowsMediaPlayer1.URL = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\myPlaylist\\" + listMusic.SelectedItem + ".mp3";
                    timer3.Interval = intervalTime;
                    resign = true;
                }
                if (isPlaying)
                {
                    axWindowsMediaPlayer1.Ctlcontrols.pause();
                    isPlaying = false;
                    timer3.Stop();
                    return;
                }
                else
                {
                    timer1.Start(); //Timer started!
                    timer2.Start();
                    timer3.Start();
                    axWindowsMediaPlayer1.Ctlcontrols.play();
                    trackBar1.Show();
                    trackBar.Show();
                    isPlaying = true;
                    return;
                }
            }
            catch
            {
                MessageBox.Show("გთხოვთ აირჩიოთ მუსიკა,Media>Open Files ან DragAndDrop-ით :)", button2.Text);
            }


        } //done

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listMusic.MouseDoubleClick += ListMusic_MouseDoubleClick;  //ორჯერ დაკლიკვით არჩეულ მუსიკაზე ლისტბოქსიდან ირთვება სასურველი ტრეკი :) EventHandler
        }

        Timer t;
        private void ListMusic_MouseDoubleClick(object sender, MouseEventArgs e) //არჩეული მუსიკის double-click-ზე იწყება მუსიკა!
        {
            count = 0;
            timer1.Dispose();
            timer2.Dispose();

            timer1.Interval = 1000;
            timer2.Interval = 1000;
            isFromPlaylist = false;
            timer3Calc();

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            try
            {
                axWindowsMediaPlayer1.URL = path + "\\myPlaylist\\" + listMusic.SelectedItem + ".mp3";
            }catch(Exception ex)
            {
                axWindowsMediaPlayer1.URL = path + "\\myPlaylist\\" + listMusic.SelectedItem + ".wav";
            }
            
            axWindowsMediaPlayer1.Ctlcontrols.play();

            str = listMusic.SelectedItem.ToString();
            strArr = str.ToCharArray();
            trackBar1.Show();
            trackBar.Show();

            resign = true;
            timer1.Start();
            timer2.Start();
            timer3.Start();

            t = new Timer();
            t.Enabled = true;
            t.Interval = 5000;
            t.Start();
            t.Tick += t_Tick;
        } //done

        string str;
        char[] strArr;
        private void t_Tick(object sender, EventArgs e) //ვცდილობ შერჩეული მუსიკის სახელი ავამოძრაო.
        {
            string musicccname = "";
            int i = 0;

            while (i < strArr.Length)
            {
                listMusic.Invoke(new Action(() =>
                {
                    musicccname += str[i];
                    listMusic.Text += musicccname;
                }));
                i++;
            }
        } //need smth...

        private void trackBar_Scroll(object sender, EventArgs e) //volume bar!
        {
            axWindowsMediaPlayer1.settings.volume = trackBar.Value; //for volume
        } //done
        public void trackBar1_Scroll(object sender, EventArgs e)  //Seeking bar!
        {
            axWindowsMediaPlayer1.Ctlcontrols.pause();
            trackBar1.ValueChanged += TrackBar1_ValueChanged;
            timer3.Dispose();
        }

        private void TrackBar1_ValueChanged(object sender, EventArgs e)//ამის მიხედვით ხდება გადახვევა მუსიკის ტრექბარზე შეხებით.
        {
            double currentIndexOfTrack = axWindowsMediaPlayer1.currentMedia.duration / trackBar1.Maximum;
            double changedPositionOfTrack = currentIndexOfTrack * trackBar1.Value;
            axWindowsMediaPlayer1.Ctlcontrols.currentPosition = changedPositionOfTrack;
            count = trackBar1.Value;
            axWindowsMediaPlayer1.Ctlcontrols.play();
            timer3.Start();
        } //need smth...

        public void label1_Click(object sender, EventArgs e)
        {
            //nothing...
        }

        public void timer1_Tick(object sender, EventArgs e) //ითვლის დროს რამდენი დარჩა ტრეკის დასრულებამდე.
        {
            try
            {
                int remain = Convert.ToInt32(axWindowsMediaPlayer1.currentMedia.duration) - Convert.ToInt32(axWindowsMediaPlayer1.Ctlcontrols.currentPosition);
                duration = Convert.ToInt32(axWindowsMediaPlayer1.currentMedia.duration);
                string minutes = (remain / 60).ToString("00");
                string seconds = (remain % 60).ToString("00.00");
                string elapsedTime = minutes + "." + seconds;
                label1.Text = elapsedTime;

                if (remain == 0)
                {
                    timer1.Dispose();
                    timer2.Dispose();
                    trackBar1.Hide();
                    trackBar.Hide();
                    currentIndex = listMusic.SelectedIndex;
                    ind = listMusic.SelectedIndex + 1;
                    listMusic.SetSelected(currentIndex + 1, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        } //done

        public void label2_Click(object sender, EventArgs e)
        {
            //nothing...
        }

        //int pos = 0;
        //bool isSelected;
        public void timer2_Tick(object sender, EventArgs e)//ითვლის დროს რამდენი გავიდა ტრეკის დაწყებიდან.
        {
            currentTime = Convert.ToInt32(axWindowsMediaPlayer1.Ctlcontrols.currentPosition);
            string minutes = (currentTime / 60).ToString("00");
            string seconds = (currentTime % 60).ToString("00.00");
            string startTime = minutes + "." + seconds;
            label2.Text = startTime;
        } //done

        int count = 0;
        int currentIndex;
        public void timer3_Tick(object sender, EventArgs e)//ტრექბარის value იცვლება ყოველ timer3-ის ტიკზე, გააჩნია ინტერვალს!
        {
            trackBar1.Maximum = 100;
            trackBar1.Minimum = 0;
            trackBar1.Value = count;
            if (count == 100)
            {
                //listMusic.ClearSelected();
                timer3.Dispose();
                try
                {
                    count = 0;
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\myPlaylist\\"; //myPlaylist ფოლდერის მისამართი სტრინგში//
                    ind = listMusic.SelectedIndex;
                    axWindowsMediaPlayer1.URL = path + droppedMusicName[ind] + ".mp3";
                    isFromPlaylist = true;
                    timer3Calc();
                    trackBar1.Show();
                    trackBar.Show();
                    axWindowsMediaPlayer1.Ctlcontrols.play();
                    timer3.Start();
                    timer2.Start();
                    timer1.Start();

                    t.Enabled = true;
                    t.Interval = 5000;
                    t.Start();
                    t.Tick += t_Tick;
                    t.Start();
                }
                catch
                {
                    trackBar1.Hide();
                    trackBar.Hide();
                    listMusic.Items.Clear();
                }
            }
            else if (count == 0 && currentTime == 0)
            {
                timer3.Start();
            }
            count++;
        } //done

        private void panelChildForm_Paint(object sender, PaintEventArgs e)
        {
            panelChildForm.AllowDrop = true;
            panelChildForm.DragDrop += new DragEventHandler(PanelChildForm_DragDrop);
            panelChildForm.DragEnter += new DragEventHandler(PanelChildForm_DragEnter);
        }

        private void PanelChildForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        List<string> droppedMusicName = new List<string>();

        private void PanelChildForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] filePath = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            for (int i = 0; i < filePath.Length; i++)
            {
                droppedMusicName.Add(Path.GetFileNameWithoutExtension(filePath[i]));
                string src = filePath[i];
                FileInfo music = new FileInfo(src);
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var dest = path + "\\myPlaylist\\" + Path.GetFileNameWithoutExtension(src) + ".mp3";
                try
                {
                    music.CopyTo(dest);
                    destination = dest;
                }
                catch
                {
                    FileInfo ff = new FileInfo(dest);
                    if (ff.Exists)
                    {
                        destination = dest;
                    }
                }
                listMusic.Items.Add(droppedMusicName[i]);
            }
        }

        private void nextSong_Click(object sender, EventArgs e) //ირთვება მომდევნო მუსიკა ლისტში!
        {
            timer1.Dispose();
            timer2.Dispose();
            timer3.Dispose();
            count = 0;
            try
            {
                string nextSong;
                if (listMusic.SelectedIndex + 1 == listMusic.Items.Count)
                {
                    nextSong = listMusic.Items[0] + ".mp3";
                    listMusic.SetSelected(0, true);
                }
                else
                {
                    nextSong = listMusic.Items[listMusic.SelectedIndex + 1] + ".mp3";
                    listMusic.SetSelected(listMusic.SelectedIndex + 1, true);
                }
                axWindowsMediaPlayer1.Ctlcontrols.pause();
                axWindowsMediaPlayer1.URL = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\myPlaylist\\" + nextSong;
                timer3Calc();
                timer1.Start();
                timer2.Start();
                timer3.Start();
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }
            catch (Exception error)
            {
                MessageBox.Show("გთხოვთ პირველ რიგში დაამატოთ playlist :)", nextSong.Name);
                MessageBox.Show(error.Message);
            }
        } //done

        private void prevSong_Click(object sender, EventArgs e) //ირთვება წინა მუსიკა ლისტში!
        {
            timer1.Dispose();
            timer2.Dispose();
            timer3.Dispose();
            count = 0;

            try
            {
                string prevSong;
                if (listMusic.SelectedIndex - 1 == -1)
                {
                    prevSong = listMusic.Items[listMusic.Items.Count - 1] + ".mp3";
                    listMusic.SetSelected(listMusic.Items.Count - 1, true);
                }
                else
                {
                    prevSong = listMusic.Items[listMusic.SelectedIndex - 1] + ".mp3";
                    listMusic.SetSelected(listMusic.SelectedIndex - 1, true);
                }
                axWindowsMediaPlayer1.Ctlcontrols.pause();
                axWindowsMediaPlayer1.URL = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\myPlaylist\\" + prevSong;
                timer3Calc();
                timer1.Start();
                timer2.Start();
                timer3.Start();
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }
            catch (Exception error)
            {
                MessageBox.Show("გთხოვთ პირველ რიგში დაამატოთ playlist :)", prevSong.Name);
            }
        } //done

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }

}
