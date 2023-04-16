using ICities;

namespace RefundMod
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class Mod : IUserMod
    {
        // https://github.com/xUMR/RefundMod
        // http://steamcommunity.com/sharedfiles/filedetails/?id=405931025

        public string Name => "Refund";
        public string Description => "Adjustable refunds and relocation costs";

        public static Data.Persistence DataPersistence { get; } = new Data.Persistence();

        public static Data Data => DataPersistence.Data;

        private OptionsUI OptionsUI { get; } = new OptionsUI(DataPersistence);

        // ReSharper disable once UnusedMember.Global
        public void OnSettingsUI(UIHelperBase helper)
        {
            OptionsUI.OnSettingsUI(helper);
        }
    }
}
