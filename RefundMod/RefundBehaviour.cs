using System;
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

        private bool _initialized;

        public Data Data;

        private SimulationManager _simulationManager;

        private int _currentDay;

        private FieldInfo _fieldInfo;

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

        private void ResetBuildIndexHistory()
        {
            if (_fieldInfo == null)
            {
                _fieldInfo = typeof(SimulationManager).GetField("m_buildIndexHistory", BindingFlags.Instance | BindingFlags.NonPublic);
            }

            var values = (uint[])_fieldInfo.GetValue(_simulationManager);
            var len = values.Length;
            for (var i = 0; i < len; i++)
                values[i] = 0;
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
