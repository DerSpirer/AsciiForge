namespace AsciiForge.Helpers
{
    internal class AsyncTimer
    {
        private Func<object?, Task> _function;
        private object? _args;
        private int _intervalMilliseconds;
        private bool _stop;

        public AsyncTimer(Func<object?, Task> function, object? args, int intervalMilliseconds)
        {
            _function = function;
            _args = args;
            _intervalMilliseconds = intervalMilliseconds;
            Start();
        }

        private async void Start()
        {
            _stop = false;
            while (!_stop)
            {
                await _function(_args);
                await Task.Delay(_intervalMilliseconds);
            }
        }

        public void Stop()
        {
            _stop = true;
        }
    }
}
