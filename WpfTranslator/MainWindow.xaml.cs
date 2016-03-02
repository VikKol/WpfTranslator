using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;

namespace WpfTranslator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int WmHotKey = 0x0312;
        private readonly IntPtr wndHandle;
        private readonly Translator translator;

        public MainWindow()
        {
            InitializeComponent();
            translator = new Translator();
            wndHandle = new WindowInteropHelper(this).Handle;
            WinApi.RegisterAppHotKey(wndHandle);
            ComponentDispatcher.ThreadPreprocessMessage += ComponentDispatcher_ThreadPreprocessMessage;
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => MessageBox.Show(args.ExceptionObject.ToString());
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            WinApi.RegisterAppHotKey(wndHandle);
            base.OnClosing(e);
        }

        private void ComponentDispatcher_ThreadPreprocessMessage(ref MSG msg, ref bool handled)
        {
            if (!handled && msg.message == WmHotKey && WinApi.AppHotKeyId == (int)msg.wParam)
            {
                HandleTranslation();
                handled = true;
            }
        }

        private void HandleTranslation()
        {
        }
    }
}
