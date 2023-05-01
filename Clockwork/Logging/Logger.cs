using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Logging
{
    public sealed class Logger
    {
        // Values
        internal bool Running = false;

        private Game _game;
        private StreamWriter _writer;

        private string _outputDirectory;


        // Properties
        public Game Game => _game;

        public string OutputDirectory => _outputDirectory;


        // Constructors
        internal Logger(Game game, ILoggerType loggerType, string? logsDirectory)
        {
            if (logsDirectory == null)
                logsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Clockwork\\Logs\\";

            Directory.CreateDirectory(logsDirectory);

            _game = game;
            _outputDirectory = logsDirectory;

            loggerType.Initialize(this);
        }


        // Static func
        //public static Logger CreateLogger(Game game)


        // Func
        internal void Start()
        {
            if (Running) return;

            _writer = new StreamWriter(_outputDirectory + GenerateFileName());

            Log(null, "Logger Started!");

            Running = true;
        }
        internal void Stop()
        {
            if (!Running) return;

            Log(null, "Logger Stopped!");

            _writer.Flush();
            _writer.Close();

            Running = false;
        }

        public void Log(object? sender, string str)
        {
            if (sender == null)
                sender = this;

            DateTime time = DateTime.Now;
            _writer.WriteLineAsync("[" + time.ToString("HH:mm:sstt") + "] " + sender.GetType().Name + " > " + str);
        }

        private string GenerateFileName()
        {
            DateTime time = DateTime.Now;
            return "log " + time.ToString("yyyy-MM-dd hh-mm-ss tt") + ".txt";
        }
    }
}
