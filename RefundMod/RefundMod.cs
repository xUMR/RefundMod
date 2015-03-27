using ICities;

namespace RefundMod
{
    public class Refund : IUserMod
    {
        public string Name { get { return "Refund"; } }
        public string Description { get { return "Removes bulldoze and relocation costs"; } }
    }

    public class RefundEconomy : EconomyExtensionBase
    {
        public override int OnGetRelocationCost(int constructionCost, int relocationCost, Service service, SubService subService, Level level)
        {
            return (int)(constructionCost * RefundBehaviour.Instance.Data.RelocateModifier);
        }

        public override int OnGetRefundAmount(int constructionCost, int refundAmount, Service service, SubService subService, Level level)
        {
            return RefundBehaviour.Instance.CanRefund
                       ? (int)(constructionCost * RefundBehaviour.Instance.Data.RefundModifier)
                       : 0;
        }
    }

    public class RefundLoading : LoadingExtensionBase
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
