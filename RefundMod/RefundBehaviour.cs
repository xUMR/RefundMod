using System.IO;
using System.Reflection;
using ColossalFramework;
using UnityEngine;

namespace RefundMod
{
    public class RefundBehaviour : MonoBehaviour
    {
        public bool CanRefund
        {
            get { return (!Data.OnlyWhenPaused || _simulationManager.SimulationPaused); }
        }

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

        private bool _initialized;

        public RefundModData Data;
        private RefundModUI _ui;

        private SimulationManager _simulationManager;

        private int _currentDay;

        private FieldInfo _fieldInfo;

        public RefundBehaviour Init()
        {
            if (_initialized)
                return this;

            _simulationManager = Singleton<SimulationManager>.instance;
            _currentDay = _simulationManager.m_currentGameTime.Day;
            _fieldInfo = typeof(SimulationManager).GetField("m_buildIndexHistory", BindingFlags.Instance | BindingFlags.NonPublic);

            try
            {
                Data = RefundModData.Deserialize();
            }
            catch (FileNotFoundException)
            {
                Data = new RefundModData();
            }
            
            _ui = gameObject.AddComponent<RefundModUI>();
            _ui.Init(Data);

            _initialized = true;

            return this;
        }

        private void ResetBuildIndexHistory()
        {
            if (_fieldInfo == null)
            {
                _fieldInfo = typeof(SimulationManager).GetField("m_buildIndexHistory", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            else
            {
                var values = (uint[])_fieldInfo.GetValue(_simulationManager);
                var len = values.Length;
                for (var i = 0; i < len; i++)
                    values[i] = 0; 
            }
        }

        void Update()
        {
            if (Data.RemoveTimeLimit && _currentDay != _simulationManager.m_currentGameTime.Day)
            {
                ResetBuildIndexHistory();
                _currentDay = _simulationManager.m_currentGameTime.Day;
            }
        }
    }
}
