using ICities;

namespace RefundMod
{
    public class Economy : EconomyExtensionBase
    {
        public override int OnGetRelocationCost(int constructionCost, int relocationCost, Service service, SubService subService, Level level)
        {
            return (int)(constructionCost * RefundBehaviour.Instance.Data.RelocateModifier);
        }

        public override int OnGetRefundAmount(int constructionCost, int refundAmount, Service service, SubService subService, Level level)
        {
            return (int)(constructionCost * RefundBehaviour.Instance.Data.RefundModifier);
        }
    }
}
