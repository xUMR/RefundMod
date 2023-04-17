using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using ICities;

namespace RefundMod
{
    // ReSharper disable once UnusedType.Global
    public sealed class Economy : EconomyExtensionBase
    {
        private EconomyManager _economySimulationManager;
        private List<IEconomyExtension> _economyExtensionsBackup;
        private List<IEconomyExtension> _economyExtensions;

        private Data ModData => Mod.Data;

        public override void OnCreated(IEconomy economy)
        {
            base.OnCreated(economy);

            if (ModData.DisableOtherEconomyMods)
            {
                RemoveOtherEconomyExtensions();
            }

            ModData.OnDisableOtherEconomyModsSet += HandleDisableOtherEconomyModsSet;
        }

        public override void OnReleased()
        {
            base.OnReleased();

            ModData.OnDisableOtherEconomyModsSet -= HandleDisableOtherEconomyModsSet;

            _economyExtensions = null;
            _economyExtensionsBackup = null;
        }

        private void HandleDisableOtherEconomyModsSet(bool value)
        {
            if (value)
            {
                RemoveOtherEconomyExtensions();
            }
            else
            {
                RestoreEconomyExtensions();
            }
        }

        private void RemoveOtherEconomyExtensions()
        {
            _economySimulationManager = Singleton<EconomyManager>.instance;

            if (_economySimulationManager.m_EconomyWrapper == null)
            {
                var coroutine = WaitForEconomyWrapperCor(
                    _economySimulationManager,
                    self => self.RemoveOtherEconomyExtensionsInternal(),
                    this);

                _economySimulationManager.StartCoroutine(coroutine);
            }
            else
            {
                RemoveOtherEconomyExtensionsInternal();
            }
        }

        private IEnumerator WaitForEconomyWrapperCor<T>(
            EconomyManager economySimulationManager,
            Action<T> callback,
            T target)
        {
            // m_EconomyWrapper is set after OnCreated callback is complete.
            while (economySimulationManager.m_EconomyWrapper == null)
            {
                yield return null;
            }

            callback?.Invoke(target);
        }

        private void RemoveOtherEconomyExtensionsInternal()
        {
            var economyWrapper = _economySimulationManager.m_EconomyWrapper;

            var value = economyWrapper
                .GetType()
                .GetField("m_EconomyExtensions", BindingFlags.Instance | BindingFlags.NonPublic)
                !.GetValue(economyWrapper);

            _economyExtensions = (List<IEconomyExtension>)value;

            Logger.Message($"Found {_economyExtensions.Count} economy extensions.");
            _economyExtensionsBackup = new List<IEconomyExtension>(_economyExtensions);

            foreach (var economyExtension in _economyExtensions)
            {
                if (economyExtension != this)
                {
                    economyExtension.OnReleased();

                    Logger.Message($"Disabled {economyExtension.GetType().FullName}");
                }
            }

            _economyExtensions.Clear();
            _economyExtensions.Add(this);
        }

        private void RestoreEconomyExtensions()
        {
            _economyExtensions = new List<IEconomyExtension>(_economyExtensionsBackup);

            foreach (var economyExtension in _economyExtensions)
            {
                if (economyExtension != this)
                {
                    economyExtension.OnCreated(economyManager);

                    Logger.Message($"Restored {economyExtension.GetType().FullName}");
                }
            }
        }

        public override int OnGetRelocationCost(
            int constructionCost,
            int relocationCost,
            Service service,
            SubService subService,
            Level level)
        {
            return (int)(constructionCost * ModData.RelocateModifier);
        }

        public override int OnGetRefundAmount(
            int constructionCost,
            int refundAmount,
            Service service,
            SubService subService,
            Level level)
        {
            return (int)(constructionCost * ModData.RefundModifier);
        }
    }
}
