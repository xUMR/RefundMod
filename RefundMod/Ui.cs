using ICities;
using UnityEngine;
using ColossalFramework.UI;
using static RefundMod.Data;

namespace RefundMod
{
    public class Ui
    {
        private Ui() { }

        const string _infoTxt =
            //------------------------------------------------------------------------------|
            "Remove Time Limit: When enabled, bulldozed buildings are always refunded.\n" +
            "Can't be enabled with \"Only When Paused\". Disabled by default.\n" +

            "\nOnly When Paused: No refunds while the game is unpaused,\n" +
            "as if the time limit is until the game is unpaused.\n" +
            "Can't be enabled with \"Remove Time Limit\". Disabled by default.\n" +

            "\nRefund Modifier: Refund percentage. Default is 75%.\n" +
            "\nRelocation Modifier: Relocation cost percentage. Default is 20%.";

        static Ui _instance = new Ui();
        public static Ui Instance => _instance;

        readonly string refundFormat = "Refund Modifier ({0}%)";
        readonly string relocateFormat = "Relocation Modifier ({0}%)";

        UICheckBox _timeLimitCheckBox;
        UICheckBox _whenPausedCheckBox;

        UISlider _refundSlider;
        UISlider _relocateSlider;

        UILabel _refundLabel;
        UILabel _relocateLabel;

        public void SettingsUi(UIHelperBase helper)
        {
            var group = helper.AddGroup("Settings");
            
            _timeLimitCheckBox = (UICheckBox)group.AddCheckbox("Remove Time Limit",
                ModData.RemoveTimeLimit, TimeLimitCheckBox);
            
            _whenPausedCheckBox = (UICheckBox)group.AddCheckbox("Only When Paused",
                ModData.OnlyWhenPaused, WhenPausedCheckBox);
            
            _refundSlider = (UISlider)group.AddSlider(string.Format(refundFormat,
                Mathf.RoundToInt(ModData.RefundModifier * 100)),
                -1, 1, 0.05f, ModData.RefundModifier, RefundSlider);
            
            _relocateSlider = (UISlider)group.AddSlider(string.Format(relocateFormat,
                Mathf.RoundToInt(ModData.RelocateModifier * 100)),
                0, 1, 0.05f, ModData.RelocateModifier, RelocateSlider);
            
            helper.AddGroup(_infoTxt);

            Logger.Message("On ui");
        }

        void TimeLimitCheckBox(bool b)
        {
            if (ModData.OnlyWhenPaused)
                _whenPausedCheckBox.SimulateClick();

            ModData.RemoveTimeLimit = b;
            ModData.Invalidate();

            ModData.Save();
        }

        void WhenPausedCheckBox(bool b)
        {
            if (ModData.RemoveTimeLimit)
                _timeLimitCheckBox.SimulateClick();

            ModData.OnlyWhenPaused = b;
            ModData.Invalidate();

            ModData.Save();
        }

        void RefundSlider(float val)
        {
            ModData.RefundModifier = val;

            if (_refundSlider && !_refundLabel)
                _refundLabel = _refundSlider.parent.Find<UILabel>("Label");
            if (_refundLabel)
                _refundLabel.text = string.Format(refundFormat, Mathf.RoundToInt(val * 100));

            ModData.Save();
        }

        void RelocateSlider(float val)
        {
            ModData.RelocateModifier = val;

            if (_relocateSlider && !_relocateLabel)
                _relocateLabel = _relocateSlider.parent.Find<UILabel>("Label");
            if (_relocateLabel)
                _relocateLabel.text = string.Format(relocateFormat, Mathf.RoundToInt(val * 100));

            ModData.Save();
        }
    }
}
