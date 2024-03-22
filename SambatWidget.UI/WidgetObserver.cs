using SambatWidget.UI.Helpers;
using SambatWidget.UI.ViewModels;
using System.Windows.Threading;

namespace SambatWidget.UI
{
    public class WidgetObserver
    {
        private readonly DispatcherTimer _timer;
        private readonly DispatcherTimer _relocationTimer;
        private readonly MainWindow _observedWindow;
        private readonly WidgetViewModel _widgetVm;
        public WidgetObserver(MainWindow observedWindow, WidgetViewModel widgetVm)
        {
            _observedWindow = observedWindow;
            _widgetVm = widgetVm;

            _timer = new DispatcherTimer { IsEnabled = true, };
            _timer.Interval = TimeSpan.FromSeconds(1);

            _relocationTimer = new DispatcherTimer { IsEnabled = App.Setting.AllowGlobalPosition, };
            _relocationTimer.Interval = TimeSpan.FromSeconds(1);
        }
        public void Observe()
        {
            _timer.Tick += _timer_Tick;
            _relocationTimer.Tick += _relocationTimer_Tick;
        }
        private void StopRelocation()
        {
            _relocationTimer.Stop();
        }
        private void StartRelocation()
        {
            _relocationTimer.Start();
        }
        private void _relocationTimer_Tick(object? sender, EventArgs e)
        {
            RunObserver(() => _observedWindow.MoveWindowToTopMost());
        }
        private void _timer_Tick(object? sender, EventArgs e)
        {
            RunObserver(() =>
            {
                RelocateWindow();
                FullScreenCheck();
                TimeZoneUpdate();
            });
        }
        private void FullScreenCheck()
        {
            bool isForegroundFullScreen = WindowHelpers.IsForegroundFullscreen();
            if (isForegroundFullScreen)
            {
                _observedWindow.Hide();
            }
            else
            {
                _observedWindow.Show();
            }
        }
        private void TimeZoneUpdate()
        {
            _widgetVm.WidgetHeaderViewModel.InitTimeZone();
        }
        private void RelocateWindow()
        {
            if (!App.Setting.AllowGlobalPosition)
            {
                StopRelocation();
                if (!_observedWindow.IsMouseDown)
                {
                    _observedWindow.CalculateWindowMoveOffset();
                }
            }
            else
            {
                StartRelocation();
            }
        }
        private void RunObserver(Action timerAction)
        {
            if (App.IsShuttingDown)
            {
                StopObserve();
                return;
            }
            timerAction?.Invoke();
        }
        private void StopObserve()
        {
            _timer.Stop();
            _timer.Tick -= _timer_Tick;

            _relocationTimer.Stop();
            _relocationTimer.Tick -= _relocationTimer_Tick;
        }
    }
}
