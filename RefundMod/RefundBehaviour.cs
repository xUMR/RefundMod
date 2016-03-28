using System;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using UnityEngine;

namespace RefundMod
{
    public class RefundBehaviour : MonoBehaviour
    {
        private static RefundBehaviour _instance;
        public static RefundBehaviour Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject();
                    _instance = go.AddComponent<RefundBehaviour>();
                    _instance.Init();
                }

                return _instance;
            }
        }

        bool _initialized;
        bool _wasPaused;
        int _currentDay;
        uint _lastBuildIndex;

        public Data Data;

        SimulationManager _simulationManager;

        uint[] m_values;
        uint[] _buildIndexHistory
        {
            get
            {
                if (m_values == null)
                {
                    m_values = (uint[])_simulationManager.GetType()
                        .GetField("m_buildIndexHistory", BindingFlags.Instance | BindingFlags.NonPublic)
                        .GetValue(_simulationManager);
                }

                return m_values;
            }
        }

        public RefundBehaviour Init()
        {
            if (_initialized)
                return this;

            _simulationManager = Singleton<SimulationManager>.instance;
            _currentDay = _simulationManager.m_currentGameTime.Day - 1;
            
            Load();

            _initialized = true;

            return this;
        }

        public void Load()
        {
            try
            {
                Data = Data.Deserialize();
            }
            catch (Exception)
            {
                Data = new Data();
                Data.Serialize();
            }
        }

        private void FillBuildIndexHistory(uint val, bool storeLastBuildIndex = true)
        {
            if (storeLastBuildIndex)
                _lastBuildIndex = _buildIndexHistory.Where(n => n != 0 && n != uint.MaxValue).Min();

            var len = _buildIndexHistory.Length;
            for (var i = 0; i < len; i++)
                _buildIndexHistory[i] = val;
        }

        private void AllowRefunds(bool useCurrentBuildIndex = true)
        {
            _buildIndexHistory[_simulationManager.m_currentFrameIndex >> 8 & 31] = useCurrentBuildIndex
                ? _simulationManager.m_currentBuildIndex
                : _lastBuildIndex;
        }

        void OnPause()
        {
            if (Data.OnlyWhenPaused)
                AllowRefunds();
        }

        void OnResume()
        {
            if (Data.OnlyWhenPaused)
                FillBuildIndexHistory(uint.MaxValue, false);
        }

        bool IsNewDay()
        {
            if (_currentDay != _simulationManager.m_currentGameTime.Day)
            {
                _currentDay = _simulationManager.m_currentGameTime.Day;
                return true;
            }

            return false;
        }
        
        void Update()
        {
            bool validated;
            if ((validated = Data.Validated()) || IsNewDay())
            {
                if (Data.OnlyWhenPaused)
                {
                    // OnlyWhenPaused has just been set in options menu
                    if (validated && _simulationManager.SimulationPaused)
                        AllowRefunds(false);
                    else
                        FillBuildIndexHistory(uint.MaxValue);
                }
                else if (Data.RemoveTimeLimit)
                {
                    FillBuildIndexHistory(0);
                }
            }

            if (!_wasPaused && _simulationManager.SimulationPaused)
                OnPause();
            else if (_wasPaused && !_simulationManager.SimulationPaused)
                OnResume();
            
            _wasPaused = _simulationManager.SimulationPaused;
        }
    }
}
