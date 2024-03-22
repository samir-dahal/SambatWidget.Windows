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
            if (!_settings.DefaultPositionSet)
            {
                _settings.SetDefaultPosition(InitPosition);
                return;
            }
            base.OnRenderSizeChanged(sizeInfo);
            try
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
            finally
            {
                CalculateAndSaveWindowMoveOffset();
            }
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            try
            {
                IsMouseDown = false;
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
        private void CalculateAndSaveWindowMoveOffset()
        {
            this.CalculateWindowMoveOffset();
            _settings.SetPosition(this.Left, this.Top);
        }
    }
}