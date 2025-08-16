using Godot;
using System;


namespace ZombieHoardGame
{
    public partial class Stopwatch : Node
    {
        public float ElapsedTime{ get{ return _timeSeconds; } }
        

        private bool _isStopped = true;
        private float _timeSeconds = 0;


        public override void _Process(double delta)
        {
            if (!_isStopped)
            {
                _timeSeconds += delta;
            }
        }

        public void Start()
        {
            _isStopped = false;
        }

        public void Stop()
        {
            _isStopped = true;
        }

        public String ElapsedTimeFormattedString()
        {
            int timeMinutes = Mathf.FloorToInt(_timeSeconds / 60);
            float timeRemainderSeconds = Mathf.Snapped(_timeSeconds - (timeMinutes * 60), 0.01f);
            return $"{timeMinutes}:{timeRemainderSeconds}";
        }
    }
}

