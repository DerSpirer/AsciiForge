using System.Diagnostics;
using AsciiForge.Engine.Audio;
using AsciiForge.Engine.Ecs;
using AsciiForge.Engine.IO;
using AsciiForge.Engine.Resources;
using AsciiForge.Helpers;

namespace AsciiForge.Engine
{
    public static class Game
    {
        public static World world { get; private set; }
        private static int _frameRate;
        public static int frameRate
        {
            get
            {
                return _frameRate;
            }
            set
            {
                if (value < 0)
                {
                    Logger.Warning($"Trying to set frame rate to a negative number: {value}");
                    throw new Exception($"Trying to set frame rate to a negative number: {value}");
                }
                if (value != _frameRate)
                {
                    _drawTimer?.Stop();
                    _frameRate = value;
                    _drawTimer = new AsyncTimer(Draw, null, (int)(1.0 / _frameRate * 1000));
                }
            }
        }
        private static int _updateRate;
        public static int updateRate
        {
            get
            {
                return _updateRate;
            }
            set
            {
                if (value < 0)
                {
                    Logger.Warning($"Trying to set update rate to a negative number: {value}");
                    throw new Exception($"Trying to set update rate to a negative number: {value}");
                }
                if (value != _updateRate)
                {
                    _updateTimer?.Stop();
                    _updateRate = value;
                    _updateTimer = new AsyncTimer(Update, null, (int)(1.0 / _updateRate * 1000));
                }
            }
        }
        private static AsyncTimer _updateTimer;
        private static AsyncTimer _drawTimer;;

        private static bool _started = false;
        private static bool _exit = false;

        public enum ExitCode
        {
            Success,
            AlreadyStarted,
            NoRooms,
            Crashed,
        }
        public static async Task<ExitCode> Commence()
        {
            Logger.Info($"Start time: {DateTime.UtcNow}");
            
            Stopwatch stopwatch = Stopwatch.StartNew();
            ExitCode exitCode = await Start();
            stopwatch.Stop();
            AudioManager.Dispose();

            Logger.Info($"Exit code: {exitCode}, Exit time: {DateTime.UtcNow}, Total run time: {stopwatch.Elapsed}");
            await Logger.Save();
            
            return exitCode;
        }
        private static async Task<ExitCode> Start()
        {
            try
            {
                if (_started)
                {
                    return ExitCode.AlreadyStarted;
                }
                _started = true;

                await ResourceManager.Load();
                if (ResourceManager.rooms.Count <= 0)
                {
                    Logger.Critical("Cannot start game with 0 rooms");
                    return ExitCode.NoRooms;
                }

                // Title screen
                world = new World(ResourceManager.rooms);
                await world.Start();

                Screen.Init();
                
                const string title = "My Game";
                Console.SetCursorPosition((int)Math.Floor(Screen.width / 2f - title.Length / 2f), (int)Math.Floor(Screen.height / 2f));
                Console.Write(title);
                await Task.Delay(3000);

                // Start update and draw loops
                updateRate = 60;
                frameRate = 60;

                while (!_exit)
                {
                    await Task.Delay(1000);
                }

                await world.Destroy();

                return ExitCode.Success;
            }
            catch (Exception exception)
            {
                Logger.Critical($"Game crashed", exception);
                return ExitCode.Crashed;
            }
        }

        private static async Task Update(object? state)
        {
            Input.Read();
            await world.Update();
        }
        private static async Task Draw(object? state)
        {
            Screen.Clear();
            await world.Draw(Screen.canvas);
            Screen.Draw();
        }

        public static void Exit()
        {
            _exit = true;
        }
    }
}
