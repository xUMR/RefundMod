using System;
using UnityEngine;
using ColossalFramework;

namespace RefundMod
{
    public sealed class EventManager : MonoBehaviour
    {
        private SimulationManager _simulationManager;

        public event Action OnPause;
        public event Action OnResume;
        public event Action OnNewTick;
        public event Action OnValidation;

#pragma warning disable CS0067
        // LCtrl + LShift + R
        public event Action OnKeyCombo;
#pragma warning restore CS0067

        private bool _isPaused;
        public bool IsPaused => _simulationManager.SimulationPaused;
        public bool IsPausedNow => !_isPaused && IsPaused;
        public bool IsResumedNow => _isPaused && !IsPaused;

        private int _currentTick; // values [0,3] are updated every 6 in-game hours.
        private DateTime CurrentGameTime => _simulationManager.m_currentGameTime;

        private bool IsNewTick
        {
            get
            {
                var quarterOfDay = CurrentGameTime.TimeOfDay.TotalHours / 6;
                var nextModTick = Convert.ToInt32(Math.Floor(quarterOfDay));
                if (_currentTick == nextModTick)
                    return false;

                _currentTick = nextModTick;
                return true;
            }
        }

        private bool IsValidated => Mod.Data.Validate();

        // ReSharper disable once UnusedMember.Local
        private bool IsKeyComboPressed =>
            Input.GetKey(KeyCode.LeftControl) &&
            Input.GetKey(KeyCode.LeftShift) &&
            Input.GetKeyDown(KeyCode.R);

        private void Start()
        {
            _simulationManager = Singleton<SimulationManager>.instance;
            _isPaused = IsPaused;

#if DEBUG
            OnPause += () => Logger.Message("OnPause");
            OnResume += () => Logger.Message("OnResume");
            OnNewTick += () => Logger.Message($"OnNewTick (CurrentGameTime={CurrentGameTime})");
            OnKeyCombo += () => Logger.Message("OnKeyCombo");
            OnValidation += () => Logger.Message("OnValidation");
#endif

            if (IsNewTick)
            {
                OnNewTick?.Invoke();
            }

            // Pause On Load (408905948) mod compatibility
            if (IsPaused)
            {
                OnPause?.Invoke();
            }
        }

        private void OnDestroy()
        {
            OnPause = null;
            OnResume = null;
            OnNewTick = null;
            OnKeyCombo = null;
            OnValidation = null;
        }

        private void Update()
        {
#if DEBUG
            if (IsKeyComboPressed)
            {
                OnKeyCombo?.Invoke();
            }
#endif
            if (IsValidated)
            {
                OnValidation?.Invoke();
            }

            if (IsNewTick)
            {
                OnNewTick?.Invoke();
            }

            if (IsResumedNow) OnResume?.Invoke();
            else if (IsPausedNow) OnPause?.Invoke();

            _isPaused = IsPaused;
        }
    }
}
