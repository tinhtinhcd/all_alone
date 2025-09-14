using Godot;
using System;
using GameGeneral;


namespace UI
{
    public partial class SettingsPage : UI.MultiPage.Page
    {
        private UserPreferences _userPreferences;
        
        private HSlider _sliderAudioMaster;
        private HSlider _sliderAudioSfx;
        private HSlider _sliderAudioMusic;
        private HSlider _sliderAudioUI;
        private HSlider _sliderMouseSensitivity;
        private CheckButton _checkBtnBorderless;
        private CheckButton _checkBtnVsync;
        private CheckButton _checkBtnFxaa;
        private OptionButton _optionBtnMsaa;

        public override void _Ready()
        {
            CacheNodeReferences();
            InitalizeSettingUIValues();
        }


        private void OnBackButtonPressed()
        {
            if(IsActive)
            {
                _userPreferences.Save();
                EmitSignal(nameof(ChangePageRequested), _backPageName);
            }
        }

        private void OnAudioSliderValueChanged(float value, int busIdx)
        {
            if (IsActive)
            {
                _userPreferences.SetAudioBusVolume((UserPreferences.AudioBus)busIdx, value);
            }
        }

        private void OnSliderMouseSenseValueChanged(float value)
        {
            if (IsActive)
            {
                _userPreferences.MouseSensitivity = value;
            }
        }

        private void OnCheckBtnBorderlessPressed()
        {
            if (IsActive)
            {
                _userPreferences.BorderlessWindow = _checkBtnBorderless.ButtonPressed;
            }
        }

        private void OnCheckBtnVsyncPressed()
        {
            if (IsActive)
            {
                _userPreferences.Vsync = _checkBtnVsync.ButtonPressed;
            }
        }

        private void OnCheckBtnFxaaPressed()
        {
            if (IsActive)
            {
                _userPreferences.Fxaa = _checkBtnFxaa.ButtonPressed;
            }
        }

        private void OnOptionBtnMsaaItemSeleted(int index)
        {
            if (IsActive)
            {
                _userPreferences.Msaa = (Viewport.Msaa)index;
            }
        }


        private void CacheNodeReferences()
        {
            _userPreferences = GetNode<UserPreferences>("/root/UserPreferences");
            
            _sliderAudioMaster = (HSlider)GetNode("SliderAudioMaster");
            _sliderAudioSfx = (HSlider)GetNode("SliderAudioSFX");
            _sliderAudioMusic = (HSlider)GetNode("SliderAudioMusic");
            _sliderAudioUI = (HSlider)GetNode("SliderAudioUI");
            _sliderMouseSensitivity = (HSlider)GetNode("SliderMouseSense");
            _checkBtnBorderless = (CheckButton)GetNode("BorderlessCheckButton");
            _checkBtnVsync = (CheckButton)GetNode("VSyncCheckButton");
            _checkBtnFxaa = (CheckButton)GetNode("FXAACheckButton");
            _optionBtnMsaa = (OptionButton)GetNode("MSAAOptionButton");
        }

        private void InitalizeSettingUIValues()
        {
            _sliderAudioMaster.Value = _userPreferences.AudioVolumes[UserPreferences.AudioBus.Master];
            _sliderAudioSfx.Value = _userPreferences.AudioVolumes[UserPreferences.AudioBus.SFX];
            _sliderAudioMusic.Value = _userPreferences.AudioVolumes[UserPreferences.AudioBus.Music];
            _sliderAudioUI.Value = _userPreferences.AudioVolumes[UserPreferences.AudioBus.UI];

            _sliderMouseSensitivity.Value = _userPreferences.MouseSensitivity;

            _checkBtnBorderless.Connect("pressed", new Callable(this, nameof(OnCheckBtnBorderlessPressed)));
            _checkBtnFxaa.Connect("pressed", new Callable(this, nameof(OnCheckBtnFxaaPressed)));
            _checkBtnVsync.Connect("pressed", new Callable(this, nameof(OnCheckBtnVsyncPressed)));
            _optionBtnMsaa.Selected = (int)_userPreferences.Msaa;
        }
    }
}

