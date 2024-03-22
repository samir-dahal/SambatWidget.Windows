using SambatWidget.UI.Helpers;
using SambatWidget.UI.Models;
using SambatWidget.UI.ViewModels;
using System.Diagnostics;
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
            this.Left = App.Setting.Position.X;
            this.Top = App.Setting.Position.Y;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            IsMouseDown = true;
            base.OnMouseLeftButtonDown(e);
            _settings.SetPosition(this.Left, this.Top);
            if (!App.Setting.LockPosition)
            {
                this.DragMove();
            }
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            this.CalculateWindowMoveOffset();
            if (_settings.IsFirstLoad)
            {
                _settings.IsFirstLoad = false;
                return;
            }
            _settings.SetPosition(this.Left, this.Top);
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            IsMouseDown = false;
            base.OnMouseLeftButtonUp(e);
            if (_settings.Position.X == this.Left && _settings.Position.Y == this.Top)
            {
                _vm.ToggleExpandCommand.Execute(null);
            }
            this.CalculateWindowMoveOffset();
            _settings.SetPosition(this.Left, this.Top);
        }
    }
}