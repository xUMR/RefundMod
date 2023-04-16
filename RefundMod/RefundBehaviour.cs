﻿using System;
using System.Linq;
using System.Reflection;

namespace RefundMod
{
    public class RefundBehaviour : IDisposable
    {
        private readonly Data _data;

        private readonly EventManager _eventManager;
        private readonly SimulationManager _simulationManager;

        private uint _lastBuildIndex;
        private uint CurrentBuildIndex => _simulationManager.m_currentBuildIndex;
        private uint CheckedBuildIndex => BuildIndexHistory[IndexToCheck];
        private uint IndexToCheck => _simulationManager.m_currentFrameIndex >> 8 & 31;

        private uint[] _buildIndexHistory;
        private uint[] BuildIndexHistory
        {
            get
            {
                if (_buildIndexHistory != null) return _buildIndexHistory;

                var value = _simulationManager
                    .GetType()
                    .GetField("m_buildIndexHistory", BindingFlags.Instance | BindingFlags.NonPublic)
                    !.GetValue(_simulationManager);

                return _buildIndexHistory = (uint[])value;
            }
        }

        public RefundBehaviour(Data data, EventManager eventManager, SimulationManager simulationManager)
        {
            _data = data;
            _eventManager = eventManager;
            _simulationManager = simulationManager;

            Subscribe();

            OnNewDay();
        }

        private void Subscribe()
        {
            _eventManager.OnPause += OnPause;
            _eventManager.OnResume += OnResume;
            _eventManager.OnNewDay += OnNewDay;
            _eventManager.OnKeyCombo += OnKeyCombo;
            _eventManager.OnValidation += OnValidation;
        }

        public void Dispose()
        {
            _eventManager.OnPause -= OnPause;
            _eventManager.OnResume -= OnResume;
            _eventManager.OnNewDay -= OnNewDay;
            _eventManager.OnKeyCombo -= OnKeyCombo;
            _eventManager.OnValidation -= OnValidation;
        }

        private void StoreBuildIndex()
        {
            _lastBuildIndex = BuildIndexHistory.Where(n => n != 0 && n != uint.MaxValue).Min();
        }

        private void RestoreBuildIndexHistory()
        {
            for (var i = 0; i < BuildIndexHistory.Length; i++)
                if (BuildIndexHistory[i] == 0 || BuildIndexHistory[i] == uint.MaxValue)
                    BuildIndexHistory[i] = _lastBuildIndex;
        }

        private void FillBuildIndexHistory(uint val)
        {
            Logger.Message("Setting to " + val);

            for (var i = 0; i < BuildIndexHistory.Length; i++)
                BuildIndexHistory[i] = val;
        }

        private void AllowRefunds(bool useCurrentBuildIndex)
        {
            BuildIndexHistory[IndexToCheck] = useCurrentBuildIndex
                ? CurrentBuildIndex
                : _lastBuildIndex;
        }

        private void OnPause()
        {
            if (_data.OnlyWhenPaused)
                AllowRefunds(true);
        }

        private void OnResume()
        {
            if (_data.OnlyWhenPaused)
            {
                FillBuildIndexHistory(uint.MaxValue);
                _lastBuildIndex++;
            }
        }

        private void OnKeyCombo()
        {
            Logger.Message(BuildIndexHistory);
            Logger.Message("last: " + _lastBuildIndex);
            Logger.Message("current: " + CurrentBuildIndex);
            Logger.Message("checked: " + CheckedBuildIndex + " @ " + IndexToCheck);

            var economyManagerHash = _simulationManager.m_ManagersWrapper.economy.GetHashCode();
            Logger.Message($"economy manager ({economyManagerHash})");
        }

        // when settings change
        private void OnValidation()
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
                RestoreBuildIndexHistory();
            }
        }

        private void OnNewDay()
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
