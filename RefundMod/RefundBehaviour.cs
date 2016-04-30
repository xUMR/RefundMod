using System.Linq;
using System.Reflection;
using ColossalFramework;
using UnityEngine;

namespace RefundMod
{
    public class RefundBehaviour : MonoBehaviour
    {
        private static RefundBehaviour _instance;
        public static RefundBehaviour Instance => _instance ? _instance
            : (_instance = new GameObject().AddComponent<RefundBehaviour>());
        
        uint _lastBuildIndex;
        uint _currentBuildIndex => _simulationManager.m_currentBuildIndex;
        uint _checkedBuildIndex => _buildIndexHistory[_indexToCheck];
        uint _indexToCheck => _simulationManager.m_currentFrameIndex >> 8 & 31;

        Data _data;

        EventManager _eventManager;
        SimulationManager _simulationManager;

        uint[] m_values;
        uint[] _buildIndexHistory => (m_values == null)
            ? (m_values = (uint[])_simulationManager.GetType()
                .GetField("m_buildIndexHistory", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(_simulationManager))
            : m_values;

        void Awake()
        {
            _data = Data.ModData;
            _eventManager = EventManager.Instance;
            _simulationManager = Singleton<SimulationManager>.instance;

            Logger.Message("RefundBehaviour Awake");
        }

        void Start()
        {
            Logger.Message("RefundBehaviour Start");

            _eventManager.OnPause += OnPause;
            _eventManager.OnResume += OnResume;
            _eventManager.OnNewDay += OnNewDay;
            _eventManager.OnKeyCombo += OnKeyCombo;
            _eventManager.OnValidation += OnValidation;

            OnNewDay();
        }

        public static void Load()
        {
            var i = Instance;
        }

        private void StoreBuildIndex()
        {
            _lastBuildIndex = _buildIndexHistory.Where(n => n != 0 && n != uint.MaxValue).Min();
        }

        private void FillBuildIndexHistory(uint val)
        {
            Logger.Message("Setting to " + val);

            var len = _buildIndexHistory.Length;
            for (var i = 0; i < len; i++)
                _buildIndexHistory[i] = val;
        }

        private void AllowRefunds(bool useCurrentBuildIndex)
        {
            _buildIndexHistory[_indexToCheck] = useCurrentBuildIndex
                ? _currentBuildIndex
                : _lastBuildIndex;
        }

        void OnPause()
        {
            if (_data.OnlyWhenPaused)
                AllowRefunds(true);
        }

        void OnResume()
        {
            if (_data.OnlyWhenPaused)
            {
                FillBuildIndexHistory(uint.MaxValue);
                _lastBuildIndex++;
            }
        }

        void OnKeyCombo()
        {
            Logger.Message(_buildIndexHistory);
            Logger.Message("last: " + _lastBuildIndex);
            Logger.Message("current: " + _currentBuildIndex);
            Logger.Message("checked: " + _checkedBuildIndex + " @ " + _indexToCheck);
        }

        // when settings change
        void OnValidation()
        {
            // OnlyWhenPaused has just been set in options menu
            if (_data.OnlyWhenPaused && _eventManager.IsPaused)
            {
                AllowRefunds(false);
            }
            else if (_data.RemoveTimeLimit)
            {
                StoreBuildIndex();
                FillBuildIndexHistory(0);
            }
            else if (!_data.OnlyWhenPaused && !_data.RemoveTimeLimit)
            {
                for (var i = 0; i < _buildIndexHistory.Length; i++)
                {
                    if (_buildIndexHistory[i] == 0 || _buildIndexHistory[i] == uint.MaxValue)
                        _buildIndexHistory[i] = _lastBuildIndex;
                }
            }
        }

        void OnNewDay()
        {
            if (_data.OnlyWhenPaused)
            {
                StoreBuildIndex();
                FillBuildIndexHistory(uint.MaxValue);
            }
            else if (_data.RemoveTimeLimit)
            {
                StoreBuildIndex();
                FillBuildIndexHistory(0);
            }
        }
    }
}
