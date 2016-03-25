using ICities;

namespace RefundMod
{
    public class Loading : LoadingExtensionBase
    {
        private RefundBehaviour _behaviour;

        public override void OnLevelLoaded(LoadMode mode)
        {
            _behaviour = RefundBehaviour.Instance;
            _behaviour.Load();
        }

        public override void OnLevelUnloading()
        {
            _behaviour.Data.Serialize();
        }
    }
}
