using SambatWidget.UI.Helpers;
using SambatWidget.UI.Models;
using SambatWidget.UI.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace SambatWidget.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly WidgetViewModel _vm;
        private readonly WidgetObserver _widgetObserver;
        private readonly WidgetWindowSetting _settings;
        public bool IsMouseDown { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            _settings = new WidgetWindowSetting();
            _vm = DataContext as WidgetViewModel;
            Loaded += MainWindow_Loaded;
            _widgetObserver = new WidgetObserver(this, _vm);
            _widgetObserver.Observe();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.HideWindowFromAltTab();
            this.SetWindowsPos();
            InitPosition();
            _settings.SetLoaded();
        }

        private void InitPosition()
        {
            this.Left = App.Setting.Position.X;
            this.Top = App.Setting.Position.Y;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            IsMouseDown = true;
            _settings.SetWindowHeight(this.ActualHeight);
            _settings.SetPosition(this.Left, this.Top);
            if (!App.Setting.LockPosition)
            {
                this.DragMove();
            }
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            if (!_settings.Loaded) return;
            else if (!_settings.DefaultPositionSet)
            {
                _settings.SetDefaultPosition(InitPosition);
            }
            else
            {
                base.OnRenderSizeChanged(sizeInfo);
            }
            if (_settings.IsRenderedAfterMouseAction)
            {
                CalculateWidgetPositionState();
                _settings.SetMouseActionRender(false);
            }
            CalculateAndSaveWindowMoveOffset();
        }

        private void CalculateWidgetPositionState()
        {
            if (!_vm.IsExpanded)
            {
                this.Top = (App.Setting.Position.Y + _settings.WindowHeight) - this.WidgetHeader.ActualHeight;
            }
            else
            {
                this.Top = (App.Setting.Position.Y + this.WidgetHeader.ActualHeight) - this.ActualHeight;
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            try
            {
                IsMouseDown = false;
                _settings.SetMouseActionRender(true);
                if (_vm.EventPopupVisible)
                {
                    _vm.HideEventPopupCommand.Execute(null);
                    return;
                }
                if (_settings.Position.X == this.Left && _settings.Position.Y == this.Top)
                {
                    _vm.ToggleExpandCommand.Execute(null);
                }
            }
            finally
            {
                CalculateAndSaveWindowMoveOffset();
            }

        }
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);
            _vm.HideEventPopupCommand.Execute(null);
            _widgetObserver.RestartScrollTimer();
            _widgetObserver.SetScrollDelta(e.Delta);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            _vm.HideEventPopupCommand.Execute(null);
            switch (e.Key)
            {
                case Key.Down:
                case Key.Left:
                    _vm.RenderPreviousCommand.Execute(null);
                    break;
                case Key.Up:
                case Key.Right:
                    _vm.RenderNextCommand.Execute(null);
                    break;
            }
        }
        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            try
            {
                _settings.SetWindowHeight(this.ActualHeight);
                _settings.SetMouseActionRender(true);
                if (_vm.EventPopupVisible)
                {
                    _vm.HideEventPopupCommand.Execute(null);
                }
                if (App.Setting.MinimizeOnLostFocus && !App.IsShuttingDown)
                {
                    _vm.MinimizeWidgetCommand.Execute(null);
                }
            }
            finally
            {
                CalculateAndSaveWindowMoveOffset();
            }
        }
        private void CalculateAndSaveWindowMoveOffset()
        {
            this.CalculateWindowMoveOffset();
            _settings.SetPosition(this.Left, this.Top);
        }
    }
}