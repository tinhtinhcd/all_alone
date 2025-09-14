using Godot;
using System;


namespace ZombieHoardGame.PlayerCharacter
{
    public partial class PlayerHUD : Control
    {
        private Label _labelGunAmmo;
        private Label _labelPoints;
        private ProgressBar _progressBarReload;
        private ProgressBar _progressBarHealth;
        private Label _labelRoundNumber;
        private Label _labelInteractiveText;
        private ColorRect _crosshair;
        private ShaderMaterial _crosshairShader;
        private TextureRect _healthEffect;
        private HealthSystem.Health _trackedHealth;
        private AudioStreamPlayer _hurtAudio;


        public override void _Ready()
        {
            CacheNodeReferences();
            _crosshairShader = (ShaderMaterial)_crosshair.Material;
            _crosshairShader.SetShaderParameter("edgeLength", GetViewport().GetVisibleRect().Size.Y);
            _progressBarReload.Hide();
        }

        public void SetHealthComponent(HealthSystem.Health playerHealth)
        {
            //_progressBarHealth.MaxValue = playerHealth.PointsMax;
            //_progressBarHealth.Value = playerHealth.Points;
            _trackedHealth = playerHealth;
            playerHealth.Connect(nameof(HealthSystem.Health.PointsChanged), new Callable(this, nameof(OnHealthValueChanged)));
        }

        public void UpdateAmmoLabel(int loaded, int remaining)
        {
            _labelGunAmmo.Text = $"{loaded} / {remaining}";
        }

        public void ShowReloadBar(float time)
        {
            Tween tween = GetTree().CreateTween();
            tween.TweenCallback(Callable.From(() => _progressBarReload.Show()));
            tween.TweenProperty(_progressBarReload, "value", 100f, time).From(0f);
            tween.TweenCallback(Callable.From(() => _progressBarReload.Hide()));
            tween.Play();
        }

        public void UpdatePointsValue(int value)
        {
            _labelPoints.Text = value.ToString();
        }

        public void UpdateRoundNumber(int number)
        {
            float tweenHalfTime = 1.2f;
            Tween tween = GetTree().CreateTween();
            tween.TweenProperty(_labelRoundNumber, "modulate", Colors.White, tweenHalfTime).From(Colors.Firebrick);
            tween.Parallel().TweenProperty(_labelRoundNumber, "rect_scale", new Vector2(2, 2), tweenHalfTime);

            tween.TweenProperty(_labelRoundNumber, "modulate", Colors.Firebrick, tweenHalfTime).From(Colors.White);
            tween.Parallel().TweenProperty(_labelRoundNumber, "rect_scale", new Vector2(1, 1), tweenHalfTime);

            tween.Play();
            _labelRoundNumber.Text = number.ToString();
        }

        public void UpdateInteractiveText(String text)
        {
            if (text == null)
            {
                _labelInteractiveText.Text = "";
                _labelInteractiveText.Hide();
            }
            else
            {
                _labelInteractiveText.Text = text;
                _labelInteractiveText.Show();
            }
        }

        public void UpdateCrosshair(float spread, float camFOV)
        {
            float viewportSizeY = GetViewport().GetVisibleRect().Size.Y;
		    float pixlesPerDegree = viewportSizeY / camFOV;
            int crosshairDiamiter = (int)Mathf.Floor(pixlesPerDegree * spread); // in pixles

            _crosshairShader.SetShaderParameter("innerDiamiter", crosshairDiamiter);
        }


        private void OnHealthValueChanged()
        {
            //_progressBarHealth.Value = value;
            float effectStrength = 1 - _trackedHealth.PointsPercentage;
            _healthEffect.Modulate = new Color(1, 1, 1, effectStrength);
            if (effectStrength > 0)
            {
                _hurtAudio.Play();
                _hurtAudio.VolumeDb = Mathf.LinearToDb(effectStrength);
            }
            else
            {
                _hurtAudio.Stop();
            }
            
        }

        private void CacheNodeReferences()
        {
            _labelGunAmmo = GetNode<Label>("GunAmmo");
            _labelPoints = GetNode<Label>("Points");
            _progressBarReload = GetNode<ProgressBar>("ReloadProgress");
            _progressBarHealth = GetNode<ProgressBar>("HealthBar");
            _labelRoundNumber = GetNode<Label>("RoundNumber");
            _labelInteractiveText = GetNode<Label>("InteractiveText");
            _crosshair = GetNode<ColorRect>("Crosshair");
            _healthEffect = GetNode<TextureRect>("HealthEffect");
            _hurtAudio = GetNode<AudioStreamPlayer>("HurtHeartBeat");
        }
    }
}

