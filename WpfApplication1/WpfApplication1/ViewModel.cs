using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApplication1
{
    class ViewModel : ViewModelBase
    {
        List<string> filter = new List<string>() { @"mp3", @"waw", @"aiff", @"flac", @"ape" };
        private int _get_selected;
        private MediaPlayer _mpBgr;
        string _playing_track;

        public int Get_selected
        {
            get
            {
                return _get_selected;
            }
            set
            {
                _get_selected = value;
                RaisePropertyChanged(() => Get_selected);
            }
        }

        public string Playing_track
        {
            get
            {
                return _playing_track;
            }
            set
            {
                _playing_track = value;
                RaisePropertyChanged(() => Playing_track);
            }
        }

        public ObservableCollection<Sound> Sounds
        {
            get;
            private set;
        }

        public ViewModel()
        {
            Sounds = new ObservableCollection<Sound>();
            _mpBgr = new MediaPlayer();
        }

        #region Commands
        ICommand _get_sound;
        ICommand _play;
        ICommand _stop;
        ICommand _pause;
        ICommand _next;
        ICommand _prev;
        ICommand _light_theme;
        ICommand _dark_theme;
        public ICommand Light_theme
        {
            get
            {
                return _light_theme ?? (_light_theme = new RelayCommand(light_theme));
            }
        }
        public ICommand Dark_theme
        {
            get
            {
                return _dark_theme ?? (_dark_theme = new RelayCommand(dark_theme));
            }
        }
        public ICommand Next
        {
            get
            {
                return _next ?? (_next = new RelayCommand(() =>
                {
                    int temp = Sounds.Count - 1;
                    if (Get_selected < temp)
                    {
                        Get_selected += 1;
                        Playing_track = Sounds[Get_selected].Path;
                    }
                    else
                    {
                        Get_selected = 0;
                        Playing_track = Sounds[Get_selected].Path;
                    }
                    Play();
                }));
            }
        }
        public ICommand Prev
        {
            get
            {
                return _prev ?? (_prev = new RelayCommand(() =>
                {
                    int temp = Sounds.Count - 1;
                    if (Get_selected > 0)
                    {
                        Get_selected -= 1;
                        Playing_track = Sounds[Get_selected].Path;
                    }
                    else
                    {
                        Get_selected = temp;
                        Playing_track = Sounds[Get_selected].Path;
                    }
                    Play();
                }));
            }
        }
        public ICommand Pause_command
        {
            get
            {
                return _pause ?? (_pause = new RelayCommand(Pause));
            }
        }
        public ICommand Get_Sound
        {
            get
            {
                return _get_sound ?? (_get_sound = new RelayCommand(Load_files));
            }
        }
        public ICommand Play_command
        {
            get
            {
                return _play ?? (_play = new RelayCommand(() =>
                {
                    Playing_track = Sounds[Get_selected].Path;
                    Play();
                }));
            }
        }
        public ICommand Stop_command
        {
            get
            {
                return _stop ?? (_stop = new RelayCommand(Stop));
            }
        }
        #endregion

        void Load_files()
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Sounds.Clear();
                foreach (string file in Directory.GetFiles(dialog.SelectedPath))
                {
                    if (filter.Exists(n => n == file.Split(new char[] { '.' }).Last().ToLower()))
                    {
                        try
                        {
                            var fileinfo = new FileInfo(file);
                            Sounds.Add(new Sound { Name = fileinfo.Name, Path = fileinfo.FullName, Size = fileinfo.Length / 1000 });
                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show(ex.Message);
                        }
                    }
                }

            }
        }

        void Play()
        {
            _mpBgr.Open(new Uri(Playing_track));
            _mpBgr.Play();
        }

        void Stop()
        {
            _mpBgr.Stop();
        }
        void Pause()
        {
            _mpBgr.Pause();
        }

        void light_theme()
        {
            var uri = new Uri("Light_theme.xaml", UriKind.Relative);
            ResourceDictionary resourceDict = System.Windows.Application.LoadComponent(uri) as ResourceDictionary;
            System.Windows.Application.Current.Resources.Clear();
            System.Windows.Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }
        void dark_theme()
        {
            var uri = new Uri("Darck_theme.xaml", UriKind.Relative);
            ResourceDictionary resourceDict = System.Windows.Application.LoadComponent(uri) as ResourceDictionary;
            System.Windows.Application.Current.Resources.Clear();
            System.Windows.Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }

    }
}
