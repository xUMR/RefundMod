using ICities;

namespace RefundMod
{
    public class Loading : LoadingExtensionBase
    {
        public override void OnLevelLoaded(LoadMode mode)
        {
            Logger.Message("OnLevelLoaded");
            RefundBehaviour.Load();
        }
    }
}
