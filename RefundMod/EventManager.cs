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
        public event Action OnNewDay;
        public event Action OnValidation;

        // LCtrl + LShift + R
        public event Action OnKeyCombo;

        private bool _isPaused;
        public bool IsPaused => _simulationManager.SimulationPaused;
        public bool IsPausedNow => !_isPaused && IsPaused;
        public bool IsResumedNow => _isPaused && !IsPaused;

        private int _currentDay;
        private int CurrentDay => _simulationManager.m_currentGameTime.Day;
        public bool IsNewDay => _currentDay != CurrentDay;

        private bool IsValidated => Mod.Data.Validate();

        private bool IsKeyComboPressed =>
            Input.GetKey(KeyCode.LeftControl) &&
            Input.GetKey(KeyCode.LeftShift) &&
            Input.GetKeyDown(KeyCode.R);

        private void Start()
        {
            _simulationManager = Singleton<SimulationManager>.instance;
            _currentDay = CurrentDay;
            _isPaused = IsPaused;

            Logger.Message("EventManager Start");
#if DEBUG
            OnPause += () => Logger.Message("OnPause");
            OnResume += () => Logger.Message("OnResume");
            OnNewDay += () => Logger.Message("OnNewDay");
            OnKeyCombo += () => Logger.Message("OnKeyCombo");
            OnValidation += () => Logger.Message("OnValidation");
#endif
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
            OnNewDay = null;
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

            if (IsNewDay)
            {
                OnNewDay?.Invoke();
                _currentDay = CurrentDay;
            }

            if (IsResumedNow) OnResume?.Invoke();
            else if (IsPausedNow) OnPause?.Invoke();

            _isPaused = IsPaused;
        }
    }
}
