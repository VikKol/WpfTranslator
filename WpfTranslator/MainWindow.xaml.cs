using System;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Media;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using Microsoft.Win32;
using Clipboard = System.Windows.Clipboard;
using MessageBox = System.Windows.MessageBox;

namespace WpfTranslator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int WmHotKey = 0x0312;

        private bool shouldClose = false;
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;

        private readonly IntPtr wndHandle;
        private readonly Translator translator;

        public MainWindow()
        {
            InitializeComponent();
            this.Hide();
            this.InitApp();

            wndHandle = new WindowInteropHelper(this).Handle;
            translator = new Translator(
                ConfigurationManager.AppSettings["MsTransaltorApiUri"],
                new AdmStsClient(
                    new AdmStsSettings(
                        ConfigurationManager.AppSettings["AzureDatamarketAccessUri"],
                        ConfigurationManager.AppSettings["ClientId"],
                        ConfigurationManager.AppSettings["ClientSecret"],
                        ConfigurationManager.AppSettings["Scope"])));

            WinApi.RegisterAppHotKey(wndHandle);
            ComponentDispatcher.ThreadPreprocessMessage += ComponentDispatcher_ThreadPreprocessMessage;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void InitApp()
        {
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("StartWithWindows", new[]
            {
                new MenuItem("Yes", OnStartupYes),
                new MenuItem("No", OnStartupNo)
            });
            trayMenu.MenuItems.Add("Exit", OnExit);
            trayIcon = new NotifyIcon
            {
                Text = Properties.Resources.AppName,
                Icon = new Icon(SystemIcons.Application, 40, 40),
                ContextMenu = trayMenu,
                Visible = true
            };

            foreach (var it in Enum.GetValues(typeof(Languages)))
            {
                this.fromLang.Items.Add(((Languages) it).ToString());
                this.toLang.Items.Add(((Languages) it).ToString());
            }
            this.fromLang.SelectedIndex = (int)Languages.en;
            this.toLang.SelectedIndex = (int)Languages.uk;
        }

        private void ComponentDispatcher_ThreadPreprocessMessage(ref MSG msg, ref bool handled)
        {
            if (!handled && msg.message == WmHotKey && WinApi.AppHotKeyId == (int)msg.wParam)
            {
                this.Show();
                this.Activate();
                HandleTranslation();
                handled = true;
            }
        }

        private async void HandleTranslation()
        {
            WinApi.SendCtrlC();
            if (Clipboard.ContainsText())
            {
                var text = Clipboard.GetText();
                translateTxt.Text = text;
                translatedTxt.Text = await SafeExec(() => translator.Translate(text, fromLang.Text, toLang.Text));
            }
        }

        private async void translateBtn_Click(object sender, RoutedEventArgs e)
        {
            var text = translateTxt.Text;
            translatedTxt.Text = await SafeExec(() => translator.Translate(text, fromLang.Text, toLang.Text));
        }

        private async void pronounceBtn_Click(object sender, RoutedEventArgs e)
        {
            var stream = await SafeExec(() => translator.Pronounce(translateTxt.Text, fromLang.Text));
            using (SoundPlayer player = new SoundPlayer(stream))
            {
                player.Play();
            }
        }

        private async Task<T> SafeExec<T>(Func<Task<T>> func)
        {
            try
            {
                return await func();
            }
            catch(WpfTranslatorException ex)
            {
                MessageBox.Show(ex.ToString());
                return default(T);
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString());
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = !shouldClose;
            this.Hide();
        }

        private void OnExit(object sender, EventArgs e)
        {
            WinApi.UnregisterAppHotKey(wndHandle);
            ComponentDispatcher.ThreadPreprocessMessage -= ComponentDispatcher_ThreadPreprocessMessage;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            trayIcon.Dispose();

            shouldClose = true;
            this.Close();
        }

        private void OnStartupYes(object sender, EventArgs e)
        {
            using (var rk = OpenRegistry())
                rk?.SetValue(Properties.Resources.AppName, Assembly.GetExecutingAssembly().FullName);
        }

        private void OnStartupNo(object sender, EventArgs e)
        {
            using (var rk = OpenRegistry())
                rk?.DeleteValue(Properties.Resources.AppName, false);
        }

        private RegistryKey OpenRegistry() => Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
    }
}
