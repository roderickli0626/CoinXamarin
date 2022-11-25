using mycoin.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace mycoin.ViewModels
{
    public class PlayPageViewModel : BaseViewModel
    {
        public Note note;
        public Plugin.SimpleAudioPlayer.ISimpleAudioPlayer audio;
        public int timerCount = 0;
        public bool endFlag = false;
        public bool closeFlag = false;

        string _titleFromPlayState, _playState, _markImageUrl, _therapyTitle, _buttonFromPlayState, _hours, _minutes;
        bool _animationState, _unFavorite, _showClose;

        public string titleFromPlayState { get => _titleFromPlayState; set => SetProperty(ref _titleFromPlayState, value); }
        public string playState { get => _playState; set => SetProperty(ref _playState, value); }
        public string markImageUrl { get => _markImageUrl; set => SetProperty(ref _markImageUrl, value); }
        public string therapyTitle { get => _therapyTitle; set => SetProperty(ref _therapyTitle, value); }
        public string buttonFromPlayState { get => _buttonFromPlayState; set => SetProperty(ref _buttonFromPlayState, value); }
        public string hours { get => _hours; set => SetProperty(ref _hours, value); }
        public string minutes { get => _minutes; set => SetProperty(ref _minutes, value); }
        public bool animationState { get => _animationState; set => SetProperty(ref _animationState, value); }
        public bool unFavorite { get => _unFavorite; set => SetProperty(ref _unFavorite, value); }
        public bool showClose { get => _showClose; set => SetProperty(ref _showClose, value); }

        public List<string> HoursList { get; private set; }
        public List<string> MinutesList { get; private set; }


        public PlayPageViewModel(int noteID)
        {
            note = App.Database.GetNoteAsync(noteID).Result;
            if (note == null) return;

            titleFromPlayState = "Application Start";
            playState = "Start";
            buttonFromPlayState = "icons8_play_48.png";
            //markImageUrl = "animation_blue_02.png";
            markImageUrl = "animation_blue_02.gif";
            //markImageUrl = "animation_green_02.png";
            //markImageUrl = "animation_green_02.gif";
            therapyTitle = note.Substance ?? "No Title";
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
            playState = "Start";
            titleFromPlayState = "Application Start";
            markImageUrl = "animation_blue_02.gif";
        });

        public ICommand playCommand => new Command(async () => {
            //var audio = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            //if (audio == null || note.WavFile == null) return;

            //audio.Load(new MemoryStream(note.WavFile));

            if (audio == null || note.WavFile == null) return;

            if (playState == "Start")
            {
                buttonFromPlayState = "icons8_square_green_48.png";
                playState = "Stop";
                titleFromPlayState = "Application Run";
                markImageUrl = "animation_green_02.gif";
                showClose = false;
                closeFlag = false;

                int hh = int.Parse(hours);
                int mm = int.Parse(minutes);
                int duration = hh * 60 + mm;
                note.Duration = duration;
                await App.Database.UpdateNoteAsync(note);

                audio.Play();

                timerCount = 0;

                Device.StartTimer(TimeSpan.FromSeconds(0.1), () =>
                {
                    if (closeFlag) return false;

                    if (playState == "Continue")
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
                            playState = "Stop";
                            titleFromPlayState = "Application Run";
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
                        playState = "Start";
                        titleFromPlayState = "Application Start";
                        markImageUrl = "animation_blue_02.gif";
                        return false;
                    }

                });
            }
            else if (playState == "Stop")
            {
                showClose = true;
                playState = "Continue";
                titleFromPlayState = "Application Start";
                buttonFromPlayState = "icons8_play_48.png";
                markImageUrl = "animation_blue_02.gif";

                audio.Pause();
            }
            else if (playState == "Continue")
            {
                buttonFromPlayState = "icons8_square_green_48.png";
                playState = "Stop";
                titleFromPlayState = "Application Run";
                markImageUrl = "animation_green_02.gif";
                showClose = false;
                audio.Play();
            }
            
        });
    }
}
