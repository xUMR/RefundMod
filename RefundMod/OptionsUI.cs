using ICities;
using UnityEngine;
using ColossalFramework.UI;

namespace RefundMod
{
    public sealed class OptionsUI
    {
        private const string INFO_TXT =
            //------------------------------------------------------------------------------|
            "Remove Time Limit: When enabled, bulldozed buildings are always refunded.\n" +
            "Can't be enabled with \"Only When Paused\". Disabled by default.\n" +

            "\nOnly When Paused: No refunds while the game is unpaused,\n" +
            "as if the time limit is until the game is unpaused.\n" +
            "Can't be enabled with \"Remove Time Limit\". Disabled by default.\n" +

            "\nDisable Other Economy Mods: Toggle this setting if any other enabled mods\n" +
            "override refund or relocation values. It may require reloading the map.\n" +
            "Disabled by default.\n" +

            "\nRefund Modifier: Refund percentage. Default is 75%.\n" +
            "\nRelocation Modifier: Relocation cost percentage. Default is 20%.";

        private const string REFUND_FORMAT = "Refund Modifier ({0}%)";
        private const string RELOCATE_FORMAT = "Relocation Modifier ({0}%)";

        private UICheckBox _timeLimitCheckBox;
        private UICheckBox _whenPausedCheckBox;

        private UISlider _refundSlider;
        private UISlider _relocateSlider;

        private UILabel _refundLabel;
        private UILabel _relocateLabel;

        private readonly Data.Persistence _persistence;

        private Data ModData => _persistence.Data;

        public OptionsUI(Data.Persistence persistence)
        {
            _persistence = persistence;
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            var group = helper.AddGroup("Settings");

            _timeLimitCheckBox = (UICheckBox)group.AddCheckbox("Remove Time Limit",
                ModData.RemoveTimeLimit, TimeLimitCheckBoxCallback);

            _whenPausedCheckBox = (UICheckBox)group.AddCheckbox("Only When Paused",
                ModData.OnlyWhenPaused, WhenPausedCheckBoxCallback);

            group.AddCheckbox("Disable Other Economy Mods",
                ModData.DisableOtherEconomyMods, DisableOthersCheckBoxCallback);

            _refundSlider = (UISlider)group.AddSlider(string.Format(REFUND_FORMAT,
                Mathf.RoundToInt(ModData.RefundModifier * 100)),
                -1, 1, 0.05f, ModData.RefundModifier, RefundSliderCallback);

            _relocateSlider = (UISlider)group.AddSlider(string.Format(RELOCATE_FORMAT,
                Mathf.RoundToInt(ModData.RelocateModifier * 100)),
                0, 1, 0.05f, ModData.RelocateModifier, RelocateSliderCallback);

            group.AddButton("Reset", ResetButtonCallback);

            helper.AddGroup(INFO_TXT);

            Logger.Message("On ui");
        }

        private void TimeLimitCheckBoxCallback(bool b)
        {
            if (ModData.OnlyWhenPaused)
                _whenPausedCheckBox.SimulateClick();

            ModData.RemoveTimeLimit = b;
            ModData.Invalidate();

            _persistence.Save();
        }

        private void WhenPausedCheckBoxCallback(bool b)
        {
            if (ModData.RemoveTimeLimit)
                _timeLimitCheckBox.SimulateClick();

            ModData.OnlyWhenPaused = b;
            ModData.Invalidate();

            _persistence.Save();
        }

        private void DisableOthersCheckBoxCallback(bool b)
        {
            ModData.DisableOtherEconomyMods = b;

            _persistence.Save();
        }

        private void RefundSliderCallback(float val)
        {
            ModData.RefundModifier = val;

            if (_refundSlider && !_refundLabel)
                _refundLabel = _refundSlider.parent.Find<UILabel>("Label");
            if (_refundLabel)
                _refundLabel.text = string.Format(REFUND_FORMAT, Mathf.RoundToInt(val * 100));

            _persistence.Save();
        }

        private void RelocateSliderCallback(float val)
        {
            ModData.RelocateModifier = val;

            if (_relocateSlider && !_relocateLabel)
                _relocateLabel = _relocateSlider.parent.Find<UILabel>("Label");
            if (_relocateLabel)
                _relocateLabel.text = string.Format(RELOCATE_FORMAT, Mathf.RoundToInt(val * 100));

            _persistence.Save();
        }

        private void ResetButtonCallback()
        {
            TimeLimitCheckBoxCallback(Data.Default.RemoveTimeLimit);
            WhenPausedCheckBoxCallback(Data.Default.OnlyWhenPaused);
            DisableOthersCheckBoxCallback(Data.Default.DisableOtherEconomyMods);
            RefundSliderCallback(Data.Default.RefundModifier);
            RelocateSliderCallback(Data.Default.RelocateModifier);

            _persistence.Save();
        }
    }
}
