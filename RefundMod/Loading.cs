using ColossalFramework;
using ICities;
using UnityEngine;

namespace RefundMod
{
    // ReSharper disable once UnusedType.Global
    public sealed class Loading : LoadingExtensionBase
    {
        private GameObject _refundModGameObject;
        private RefundBehaviour _refund;

        public override void OnLevelLoaded(LoadMode mode)
        {
            Logger.Message("OnLevelLoaded");

            _refundModGameObject = new GameObject("RefundMod");

            var eventManager = _refundModGameObject.AddComponent<EventManager>();
            var data = Mod.DataPersistence.Data;
            var simulationManager = Singleton<SimulationManager>.instance;

            _refund = new RefundBehaviour(data, eventManager, simulationManager);
        }

        public override void OnLevelUnloading()
        {
            _refund?.Dispose();

            if (_refundModGameObject)
            {
                Object.Destroy(_refundModGameObject);
            }
        }
    }
}
