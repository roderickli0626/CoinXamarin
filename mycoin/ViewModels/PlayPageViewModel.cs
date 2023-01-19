using mycoin.Extensions;
using mycoin.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XF.Material.Forms.Resources;
using XF.Material.Forms.UI.Dialogs.Configurations;
using XF.Material.Forms.UI.Dialogs;

namespace mycoin.ViewModels
{
    public class PlayPageViewModel : BaseViewModel
    {
        public Note note;
        public Plugin.SimpleAudioPlayer.ISimpleAudioPlayer audio;
        public int timerCount = 0;
        public bool endFlag = false;
        public bool closeFlag = false;

        string _titleFromPlayState, _playState, _markImageUrl, _therapyTitle, _buttonFromPlayState, _hours, _minutes, _currentTitle, _lengthLbl;
        bool _animationState, _unFavorite, _showClose;

        public string titleFromPlayState { get => _titleFromPlayState; set => SetProperty(ref _titleFromPlayState, value); }
        public string playState { get => _playState; set => SetProperty(ref _playState, value); }
        public string markImageUrl { get => _markImageUrl; set => SetProperty(ref _markImageUrl, value); }
        public string therapyTitle { get => _therapyTitle; set => SetProperty(ref _therapyTitle, value); }
        public string buttonFromPlayState { get => _buttonFromPlayState; set => SetProperty(ref _buttonFromPlayState, value); }
        public string hours { get => _hours; set => SetProperty(ref _hours, value); }
        public string minutes { get => _minutes; set => SetProperty(ref _minutes, value); }
        public string CurrentTitle { get => _currentTitle; set => SetProperty(ref _currentTitle, value); }
        public string LengthLbl { get => _lengthLbl; set => SetProperty(ref _lengthLbl, value); }
        public bool animationState { get => _animationState; set => SetProperty(ref _animationState, value); }
        public bool unFavorite { get => _unFavorite; set => SetProperty(ref _unFavorite, value); }
        public bool showClose { get => _showClose; set => SetProperty(ref _showClose, value); }

        public List<string> HoursList { get; private set; }
        public List<string> MinutesList { get; private set; }


        public PlayPageViewModel(int noteID)
        {
            note = App.Database.GetNoteAsync(noteID).Result;
            if (note == null) return;

            titleFromPlayState = GlobalConstants.LangGUI.GetValueOrDefault("Application Start", "Application Start");
            playState = GlobalConstants.LangGUI.GetValueOrDefault("Start", "Start");
            buttonFromPlayState = "icons8_play_48.png";
            //markImageUrl = "animation_blue_02.png";
            markImageUrl = "animation_blue_02.png";
            markImageUrl = "animation_blue_02.gif";
            //markImageUrl = "animation_green_02.png";
            //markImageUrl = "animation_green_02.gif";
            therapyTitle = GlobalConstants.SubTexts.GetValueOrDefault(note.SubstanceID, note.Substance ?? "No Title");
            CurrentTitle = GlobalConstants.LangGUI.GetValueOrDefault("Your Current Therapy", "Your Current Therapy");
            LengthLbl = GlobalConstants.LangGUI.GetValueOrDefault("Length of Time", "Length of Time");
            animationState = true;
            unFavorite = !note.Isfavorite;
            showClose = false;
            HoursList = new List<string>();
            MinutesList = new List<string>();
            for (int i = 0; i < 60; i++)
            {
                string ii = i.ToString("D2");
                if (i < 24) HoursList.Add(ii);
                MinutesList.Add(ii);
            }
            hours = (note.Duration / 60).ToString("D2");
            minutes = (note.Duration % 60).ToString("D2");

            //Init AudioPlayer
            InitAudioPlayer();
        }

        void InitAudioPlayer ()
        {
            audio = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            if (audio == null || note.WavFile == null) return;
            audio.Load(new MemoryStream(note.WavFile));
            audio.Loop = true;
            audio.PlaybackEnded += EndAudioPlaying;
        }

        void EndAudioPlaying(object sender, EventArgs e)
        {
            //playState = "Start";
            //buttonFromPlayState = "icons8_play_48.png";
            showClose=false;
            //titleFromPlayState = "Application Start";
            //markImageUrl = "animation_blue_02.gif";
            endFlag = true;
        }

        public ICommand closeCommand => new Command(async () =>
        {
            closeFlag = true;
            audio.Play();
            audio.Stop();
            buttonFromPlayState = "icons8_play_48.png";
            playState = GlobalConstants.LangGUI.GetValueOrDefault("Start", "Start");
            titleFromPlayState = GlobalConstants.LangGUI.GetValueOrDefault("Application Start", "Application Start");
            markImageUrl = "animation_blue_02.png";
            markImageUrl = "animation_blue_02.gif";
        });

        public ICommand playCommand => new Command(async () => {
            //var audio = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            //if (audio == null || note.WavFile == null) return;

            //audio.Load(new MemoryStream(note.WavFile));

            if (audio == null || note.WavFile == null)
            {
                var alertDialogConfiguration = new MaterialAlertDialogConfiguration()
                {
                    BackgroundColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.SURFACE),
                    TitleTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_SURFACE),
                    MessageTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_SURFACE),
                    //TintColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_BACKGROUND),
                    TintColor = Color.FromHex("#018BD3"),
                    CornerRadius = 30,
                    ScrimColor = Color.FromHex("#232F34").MultiplyAlpha(0.32),
                    ButtonAllCaps = false
                };
                var result = await MaterialDialog.Instance.ConfirmAsync(GlobalConstants.LangGUI.GetValueOrDefault("There is no content in the Substance", "There is no content in the Substance"),
                GlobalConstants.LangGUI.GetValueOrDefault("Warning", "Warning"), GlobalConstants.LangGUI.GetValueOrDefault("OK", "OK"), "", alertDialogConfiguration);

                return;
            }

            try {
                if (playState == GlobalConstants.LangGUI.GetValueOrDefault("Start", "Start"))
                {
                    buttonFromPlayState = "icons8_square_green_48.png";
                    playState = GlobalConstants.LangGUI.GetValueOrDefault("Stop", "Stop");
                    titleFromPlayState = GlobalConstants.LangGUI.GetValueOrDefault("Application Run", "Application Run");
                    markImageUrl = "animation_green_02.png";
                    markImageUrl = "animation_green_02.gif";
                    showClose = false;
                    closeFlag = false;

                    int hh = int.Parse(hours);
                    int mm = int.Parse(minutes);
                    int duration = hh * 60 + mm;
                    note.Duration = duration;
                    note.PlayDateTime = DateTime.Now;
                    await App.Database.UpdateNoteAsync(note);

                    audio.Play();

                    timerCount = 0;

                    Device.StartTimer(TimeSpan.FromSeconds(0.1), () =>
                    {
                        if (closeFlag) return false;

                        if (playState == GlobalConstants.LangGUI.GetValueOrDefault("Continue", "Continue"))
                        {
                            return true;
                        }
                        else
                        {
                            timerCount++;
                        }
                        double total = timerCount * 0.1;
                        if (note.Duration * 60 > total)
                        {
                            if (endFlag)
                            {
                                buttonFromPlayState = "icons8_square_green_48.png";
                                playState = GlobalConstants.LangGUI.GetValueOrDefault("Stop", "Stop");
                                titleFromPlayState = GlobalConstants.LangGUI.GetValueOrDefault("Application Run", "Application Run");
                                showClose = false;

                                audio.Play();
                                endFlag = false;
                            }

                            return true;
                        }
                        else
                        {
                            audio.Stop();
                            buttonFromPlayState = "icons8_play_48.png";
                            playState = GlobalConstants.LangGUI.GetValueOrDefault("Start", "Start");
                            titleFromPlayState = GlobalConstants.LangGUI.GetValueOrDefault("Application Start", "Application Start");
                            markImageUrl = "animation_blue_02.png";
                            markImageUrl = "animation_blue_02.gif";
                            return false;
                        }

                    });
                }
                else if (playState == GlobalConstants.LangGUI.GetValueOrDefault("Stop", "Stop"))
                {
                    showClose = true;
                    playState = GlobalConstants.LangGUI.GetValueOrDefault("Continue", "Continue");
                    titleFromPlayState = GlobalConstants.LangGUI.GetValueOrDefault("Application Start", "Application Start");
                    buttonFromPlayState = "icons8_play_48.png";
                    markImageUrl = "animation_blue_02.png";
                    markImageUrl = "animation_blue_02.gif";

                    audio.Pause();
                }
                else if (playState == GlobalConstants.LangGUI.GetValueOrDefault("Continue", "Continue"))
                {
                    buttonFromPlayState = "icons8_square_green_48.png";
                    playState = GlobalConstants.LangGUI.GetValueOrDefault("Stop", "Stop");
                    titleFromPlayState = GlobalConstants.LangGUI.GetValueOrDefault("Application Run", "Application Run");
                    markImageUrl = "animation_green_02.png";
                    markImageUrl = "animation_green_02.gif";
                    showClose = false;
                    audio.Play();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
        });
    }
}
