using SambatWidget.UI.Helpers;
using SambatWidget.UI.ViewModels;
using System.Windows.Threading;

namespace SambatWidget.UI
{
    public class WidgetObserver
    {
        private readonly DispatcherTimer _timer;
        private readonly DispatcherTimer _relocationTimer;
        private readonly DispatcherTimer _scrollTimer;
        private readonly MainWindow _observedWindow;
        private readonly WidgetViewModel _widgetVm;
        private bool _isScrolling;
        private int _scrollDelta;
        private WindowZOrder? _currentZOrder = null;
        public WidgetObserver(MainWindow observedWindow, WidgetViewModel widgetVm)
        {
            _observedWindow = observedWindow;
            _widgetVm = widgetVm;

            _timer = new DispatcherTimer { IsEnabled = true, };
            _timer.Interval = TimeSpan.FromSeconds(1);

            _scrollTimer = new DispatcherTimer();
            _scrollTimer.Interval = TimeSpan.FromMilliseconds(100);

            _relocationTimer = new DispatcherTimer { IsEnabled = App.Setting.AllowGlobalPosition, };
            _relocationTimer.Interval = TimeSpan.FromSeconds(1);
        }
        public void SetScrollDelta(int delta)
        {
            _scrollDelta = delta;
        }
        public void RestartScrollTimer()
        {
            _isScrolling = true;
            _scrollTimer.Stop();
            _scrollTimer.Start();
        }
        public void Observe()
        {
            _timer.Tick += _timer_Tick;
            _relocationTimer.Tick += _relocationTimer_Tick;
            _scrollTimer.Tick += _scrollTimer_Tick;
        }

        private void _scrollTimer_Tick(object? sender, EventArgs e)
        {
            // Stop the timer
            _scrollTimer.Stop();

            // Perform the action after the scrolling has stopped
            if (!_isScrolling)
            {
                return;
            }
            // Perform the action here
            if (_scrollDelta < 0)
            {
                //scrolled down
                _widgetVm.RenderNextCommand.Execute(null);
            }
            else
            {
                //scrolled up
                _widgetVm.RenderPreviousCommand.Execute(null);
            }
            _isScrolling = false;
        }

        private void StopRelocation()
        {
            _relocationTimer.Stop();
        }
        private void StartRelocation()
        {
            _relocationTimer.Start();
        }
        /// <summary>
        /// this is only for Free Positioning / Gloabl Position as this will be on top of everything (even taskbar)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _relocationTimer_Tick(object? sender, EventArgs e)
        {
            _observedWindow.MoveWindowToTopMost();
        }
        private void _timer_Tick(object? sender, EventArgs e)
        {
            ApplyWindowZOrder();
            RelocateWindow();
            FullScreenCheck();
            TimeZoneUpdate();
        }

        private void ApplyWindowZOrder()
        {
            var desiredZOrder = App.Setting.StickToDesktop ? WindowZOrder.BOTTOM_MOST : WindowZOrder.TOP_MOST;

            if (_currentZOrder != desiredZOrder)
            {
                _observedWindow.SetWindowsPos(desiredZOrder);
                _currentZOrder = desiredZOrder;
            }
        }

        private void FullScreenCheck()
        {
            if (App.Setting.StickToDesktop) return;
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
    }
}
