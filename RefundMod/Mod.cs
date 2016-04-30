using ICities;

namespace RefundMod
{
    public class Mod : IUserMod
    {
        // https://github.com/xUMR/RefundMod
        // http://steamcommunity.com/sharedfiles/filedetails/?id=405931025

        public string Name => "Refund"; 
        public string Description => "Adjustable refunds and relocation costs";
        
        public delegate void SettingsUiDelegate(UIHelperBase helper);
        readonly SettingsUiDelegate _uiDelegate = Ui.Instance.SettingsUi;

        public void OnSettingsUI(UIHelperBase helper)
        {
            _uiDelegate(helper);
        }
    }
}
