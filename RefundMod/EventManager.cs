using UnityEngine;
using ColossalFramework;

namespace RefundMod
{
    public class EventManager : MonoBehaviour
    {
        static EventManager _instance;
        public static EventManager Instance => _instance ? _instance
            : (_instance = new GameObject().AddComponent<EventManager>());
        
        SimulationManager _simulationManager;

        public event PauseHandler OnPause;
        public delegate void PauseHandler();

        public event ResumeHandler OnResume;
        public delegate void ResumeHandler();

        bool _wasPaused;
        public bool IsPaused => _simulationManager.SimulationPaused;

        public event NewDayHandler OnNewDay;
        public delegate void NewDayHandler();

        public bool IsNewDay => (_lastDay != _currentDay);

        int _lastDay;
        int _currentDay => _simulationManager.m_currentGameTime.Day;

        public event ValidationHandler OnValidation;
        public delegate void ValidationHandler();

        public bool Validated => Data.ModData.Validated();
        
        // LCtrl + LShift + R
        public event KeyComboHandler OnKeyCombo;
        public delegate void KeyComboHandler();

        void Start()
        {
            _simulationManager = Singleton<SimulationManager>.instance;
            _lastDay = _currentDay;
            _wasPaused = IsPaused;
            
            Logger.Message("EventManager Start");
#if DEBUG
            OnPause += () => { Logger.Message("OnPause"); };
            OnResume += () => { Logger.Message("OnResume"); };
            OnNewDay += () => { Logger.Message("OnNewDay"); };
            OnKeyCombo += () => { Logger.Message("OnKeyCombo"); };
            OnValidation += () => { Logger.Message("OnValidation"); };
#endif
            // Pause On Load (408905948) mod compatibility
            if (IsPaused)
            {
                OnPause();
            }
        }

        void Update()
        {
#if DEBUG
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
            {
                OnKeyCombo();
            }
#endif
            if (Validated)
            {
                OnValidation();
            }

            if (IsNewDay)
            {
                OnNewDay();
                _lastDay = _currentDay;
            }

            if (_wasPaused && !IsPaused)
                OnResume();
            else if (!_wasPaused && IsPaused)
                OnPause();

            _wasPaused = IsPaused;
        }
    }
}
