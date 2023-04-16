using System;
using static ColossalFramework.Plugins.PluginManager;

namespace RefundMod
{
    public static class Logger
    {
        private const bool ENABLED =
#if DEBUG
        true;
#else
        false;
#endif

        private const string LABEL = "(refundmod): ";

        public static void Message<T>(T o) => Log(MessageType.Message, o);
        public static void Warning<T>(T o) => Log(MessageType.Warning, o);
        public static void Error<T>(T o) => Log(MessageType.Error, o);

#pragma warning disable S1144 // Unused private types or members should be removed
#pragma warning disable CS0162 // Unreachable code detected
        private static void Log<T>(MessageType messageType, T o)
        {
            if (!ENABLED) return;

            string message;
            if (o is Array array)
            {
                var strBuilder = new System.Text.StringBuilder();
                foreach (var obj in array)
                {
                    strBuilder.Append(obj);
                    strBuilder.Append(' ');
                }

                message = strBuilder.ToString();
            }
            else
            {
                message = o.ToString();
            }

            DebugOutputPanel.AddMessage(messageType, LABEL + message);
        }
#pragma warning restore CS0162 // Unreachable code detected
#pragma warning restore S1144 // Unused private types or members should be removed
    }
}
