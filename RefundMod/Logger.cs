using ColossalFramework.Plugins;

namespace RefundMod
{
    public static class Logger
    {
        const bool _enabled = false;
        const string _label = "(refundmod): ";

#pragma warning disable CS0162 // Unreachable code detected
        public static void Message(string str)
        {
            if (!_enabled) return;

            DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, _label + str);
        }

        public static void Message(object[] args, string join = " ")
        {
            if (!_enabled) return;

            var strBuilder = new System.Text.StringBuilder();
            foreach (var obj in args)
            {
                strBuilder.Append(obj);
                strBuilder.Append(join);
            }

            Message(strBuilder.ToString());
        }

        public static void Warning(string str)
        {
            if (!_enabled) return;

            DebugOutputPanel.AddMessage(PluginManager.MessageType.Warning, _label + str);
        }

        public static void Error(string str)
        {
            if (!_enabled) return;

            DebugOutputPanel.AddMessage(PluginManager.MessageType.Error, _label + str);
        }
#pragma warning restore CS0162 // Unreachable code detected
    }
}
