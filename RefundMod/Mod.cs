using ICities;
using UnityEngine;
using ColossalFramework.UI;

namespace RefundMod
{
    public class Mod : IUserMod
    {
        public string Name { get { return "Refund"; } }

        const string _infoTxt =
            //------------------------------------------------------------------------------|
            "Remove Time Limit: When enabled, bulldozed buildings are always refunded.\n" +
            "Disabled by default.\n" +
            
            "\nRefund Modifier: Refund percentage. Default is 75%.\n" +
            "\nRelocation Modifier: Relocation cost percentage. Default is 20%.";

        public void OnSettingsUI(UIHelperBase helper)
        {
            var options = RefundBehaviour.Instance.Data;

            var group = helper.AddGroup("Settings");

            var refundFormat = "Refund Modifier ({0}%)";
            var relocateFormat = "Relocation Modifier ({0}%)";

            UISlider refundSlider = null;
            UISlider relocateSlider = null;

            UILabel refundLabel = null;
            UILabel relocateLabel = null;

            group.AddCheckbox("Remove Time Limit", options.RemoveTimeLimit,
                (b) =>
                {
                    RefundBehaviour.Instance.Data.RemoveTimeLimit = b;

                    RefundBehaviour.Instance.Data.Serialize();
                });

            refundSlider = (UISlider)group.AddSlider(string.Format(refundFormat, Mathf.RoundToInt(options.RefundModifier * 100)), -1, 1, 0.05f, options.RefundModifier,
                (val) =>
                {
                    RefundBehaviour.Instance.Data.RefundModifier = val;

                    if (refundSlider && !refundLabel)
                        refundLabel = refundSlider.parent.Find<UILabel>("Label");
                    if (refundLabel)
                        refundLabel.text = string.Format(refundFormat, Mathf.RoundToInt(val * 100));

                    RefundBehaviour.Instance.Data.Serialize();
                });

            relocateSlider = (UISlider)group.AddSlider(string.Format(relocateFormat, Mathf.RoundToInt(options.RelocateModifier * 100)), 0, 1, 0.05f, options.RelocateModifier,
                (val) =>
                {
                    RefundBehaviour.Instance.Data.RelocateModifier = val;

                    if (relocateSlider && !relocateLabel)
                        relocateLabel = relocateSlider.parent.Find<UILabel>("Label");
                    if (relocateLabel)
                        relocateLabel.text = string.Format(relocateFormat, Mathf.RoundToInt(val * 100));

                    RefundBehaviour.Instance.Data.Serialize();
                });

            helper.AddGroup(_infoTxt);
        }
    }
}
