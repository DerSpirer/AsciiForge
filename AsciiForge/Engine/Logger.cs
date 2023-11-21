using System.Text.Json;
using System.Text.Json.Serialization;

namespace AsciiForge.Engine
{
    public static class Logger
    {
        private static readonly List<Log> _logs = new List<Log>();

        public static void Info(string message, Exception? exception = null) => _logs.Add(new Log(Type.Info, message, exception));
        public static void Warning(string message, Exception? exception = null) => _logs.Add(new Log(Type.Warning, message, exception));
        public static void Error(string message, Exception? exception = null) => _logs.Add(new Log(Type.Error, message, exception));
        public static void Critical(string message, Exception? exception = null) => _logs.Add(new Log(Type.Critical, message, exception));

        internal static async Task Save()
        {
            try
            {
                if (_logs.Count > 0)
                {
                    string directory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
                    Directory.CreateDirectory(directory);
                    string fileName = DateTime.UtcNow.ToString("yyyy-MM-ddTHH-mm-ss") + ".json";
                    string path = Path.Combine(directory, fileName);
                    using FileStream fileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write);
                    JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
                    await JsonSerializer.SerializeAsync(fileStream, _logs, typeof(List<Log>), options);
                }
            }
            catch (Exception)
            {
            }
        }

        public enum Type
        {
            Info,
            Warning,
            Error,
            Critical
        }
        private struct Log
        {
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public Type type { get; set; }
            public string message { get; set; }
            public Exception? exception { get; set; }
            public DateTime time { get; set; }
            
            public Log(Type type, string message, Exception? exception)
            {
                this.type = type;
                this.message = message;
                this.exception = exception;
                this.time = DateTime.UtcNow;
            }
        }
    }
}
