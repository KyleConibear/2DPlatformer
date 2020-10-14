using System.Diagnostics;
using System.Text;

namespace KyleConibear
{
    public static class Logger
    {
        // To be used in a comparison with the new log being created. If they are the same don't print the same log twice
        private static string preivousLog = string.Empty;

        public enum Type
        {
            Message = 0,
            Warning = 1,
            Error = 2
        }

        public static void Log(bool isLogging, Type messageType = Type.Message, string message = "Method has executed.", bool isSinglePrint = true)
        {
            if (isLogging == false && messageType != Type.Error)
                return;

            Log(messageType, message, isSinglePrint);
        }

        public static void Log(Type messageType = Type.Message, string message = "Method has executed.", bool isSinglePrint = true)
        {
            StackFrame frame = new StackFrame(1);

            StringBuilder log = new StringBuilder();

            string callers_method_name = frame.GetMethod().Name;

            string callers_class_name = frame.GetMethod().DeclaringType.Name;

            // Create signature
            string signature = $"{callers_class_name}:{callers_method_name}-";

            log.Append(signature + message);

            if (isSinglePrint)
            {
                if (log.ToString() != preivousLog)
                {
                    preivousLog = log.ToString();
                    PrintLogByType(messageType, log.ToString());
                }
            }
            else
            {
                PrintLogByType(messageType, log.ToString());
            }
        }

        private static void PrintLogByType(Type type, string message)
        {
            switch (type)
            {
                case Type.Message:
                UnityEngine.Debug.Log(message);
                break;
                case Type.Warning:
                UnityEngine.Debug.LogWarning(message);
                break;
                case Type.Error:
                UnityEngine.Debug.LogError(message);
                break;
            }
        }
    }
}
