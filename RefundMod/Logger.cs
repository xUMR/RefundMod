using System;
using static ColossalFramework.Plugins.PluginManager;

namespace RefundMod
{
    public static class Logger
    {
#if DEBUG
        const bool _enabled = true;
#else
        const bool _enabled = false;
#endif
        const string _label = "(refundmod): ";

        public static void Message<T>(T o, params T[] args) => Log(MessageType.Message, o, args);
        public static void Warning<T>(T o, params T[] args) => Log(MessageType.Warning, o, args);
        public static void Error<T>(T o, params T[] args) => Log(MessageType.Error, o, args);

#pragma warning disable S1144 // Unused private types or members should be removed
#pragma warning disable CS0162 // Unreachable code detected
        static void Log<T>(MessageType messageType, T o, params T[] args)
        {
            if (!_enabled) return;

            string message;
            if (o is Array)
            {
                var array = o as Array;
                var strBuilder = new System.Text.StringBuilder();
                foreach (var obj in array)
                {
                    strBuilder.Append(obj);
                    strBuilder.Append(' ');
                }

                message = strBuilder.ToString();
            }
            else if (args.Length > 0)
            {
                var strBuilder = new System.Text.StringBuilder(o.ToString());
                foreach (var obj in args)
                {
                    strBuilder.Append(obj);
                    strBuilder.Append(' ');
                }

                message = strBuilder.ToString();
            }
            else
                message = o.ToString();

            DebugOutputPanel.AddMessage(messageType, _label + message);
        }
#pragma warning restore CS0162 // Unreachable code detected
#pragma warning restore S1144 // Unused private types or members should be removed
    }
}
