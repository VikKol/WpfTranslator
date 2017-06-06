using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using Clipboard = System.Windows.Clipboard;
using MessageBox = System.Windows.MessageBox;
using WindowsInput;

namespace WpfTranslator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int DelayMs = 200;
        const int WmHotKey = 0x0312;

        private bool shouldClose = false;
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;

        private readonly IntPtr wndHandle;
        private readonly Translator translator;
        private readonly SpellCheckService spellCheckService;
        private readonly InputSimulator simulator = new InputSimulator();

        private string fromLangCode => fromLang.SelectedValue == null ? MainLanguages.DefaultFromLanguage.Code : (fromLang.SelectedValue as Language)?.Code;
        private string toLangCode => toLang.SelectedValue == null ? MainLanguages.DefaultToLanguage.Code : (toLang.SelectedValue as Language)?.Code;

        public MainWindow()
        {
            InitializeComponent();
            this.Hide();
            this.Init();

            //var windo = new SpellCheckWindow();
            //windo.Show();
            //windo.Activate();

            wndHandle = new WindowInteropHelper(this).Handle;
            translator = new Translator(
                ConfigurationManager.AppSettings["MsTransaltorApiUri"],
                new StsClient(new StsSettings(ConfigurationManager.AppSettings["StsUri"], ConfigurationManager.AppSettings["SubscribtionKey"])));
            //spellCheckService = new SpellCheckService(ConfigurationManager.AppSettings["MashapeKey"]);
            DataContext = new MainViewModel(translator);

            WinApi.RegisterAppHotKey(wndHandle);
            ComponentDispatcher.ThreadPreprocessMessage += ComponentDispatcher_ThreadPreprocessMessage;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void Init()
        {
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Show", OnShow);
            trayMenu.MenuItems.Add("Start with Windows", new[]
            {
                new MenuItem("Yes", (e, s) => WinApi.AddToStartup()),
                new MenuItem("No", (e, s) => WinApi.DeleteFromStartup())
            });
            trayMenu.MenuItems.Add("Exit", OnExit);
            trayIcon = new NotifyIcon
            {
                Text = Properties.Resources.AppName,
                Icon = new Icon(Properties.Resources.WpfTranslator, 40, 40),
                ContextMenu = trayMenu,
                Visible = true
            };
        }

        private void ComponentDispatcher_ThreadPreprocessMessage(ref MSG msg, ref bool handled)
        {
            if (!handled && msg.message == WmHotKey)
            {
                switch((int)msg.wParam)
                {
                    case WinApi.TranslateHotKeyId:
                    {
                        DoTranslation();
                        handled = true;
                        break;
                    }
                    case WinApi.PronounceHotKeyId:
                    {
                        Speak();
                        handled = true;
                        break;
                    }
                }
            }
        }

        private async void DoTranslation()
        {
            KeystrokeCtrlC(DelayMs);
            this.Show();
            this.Activate();

            if (Clipboard.ContainsText())
            {
                var text = Clipboard.GetText();
                translateTxt.Text = text;
                translatedTxt.Text = await SafeExec(() => translator.TranslateAsync(text, fromLangCode, toLangCode));
            }
        }

        private async void Speak()
        {
            KeystrokeCtrlC(DelayMs);

            if (Clipboard.ContainsText())
            {
                var text = Clipboard.GetText();
                var stream = await SafeExec(() => translator.PronounceAsync(text, fromLangCode));
                using (SoundPlayer player = new SoundPlayer(stream))
                {
                    player.Play();
                }
            }
        }

        private void KeystrokeCtrlC(int delayMs)
        {
            simulator.Keyboard.Sleep(delayMs);
            simulator.Keyboard.ModifiedKeyStroke(
                WindowsInput.Native.VirtualKeyCode.CONTROL,
                WindowsInput.Native.VirtualKeyCode.VK_C);

            simulator.Keyboard.Sleep(delayMs);
            simulator.Keyboard.ModifiedKeyStroke(
                WindowsInput.Native.VirtualKeyCode.CONTROL,
                WindowsInput.Native.VirtualKeyCode.VK_C);
        }

        private async void translateBtn_Click(object sender, RoutedEventArgs e)
        {
            var text = translateTxt.Text;
            translatedTxt.Text = await SafeExec(() => translator.TranslateAsync(text, fromLangCode, toLangCode));
        }

        private async void pronounceBtn_Click(object sender, RoutedEventArgs e)
        {
            var stream = await SafeExec(() => translator.PronounceAsync(translateTxt.Text, fromLangCode));
            using (SoundPlayer player = new SoundPlayer(stream))
            {
                player.Play();
            }
        }

        private void switchBtn_Click(object sender, RoutedEventArgs e)
        {
            object tmp = this.fromLang.SelectedItem;
            this.fromLang.SelectedItem = this.toLang.SelectedItem;
            this.toLang.SelectedItem = tmp;
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (this.IsActive && e.Key == Key.Escape)
            {
                this.translateTxt.Text = "";
                this.translatedTxt.Text = "";
                this.Hide();
            }
        }

        private void translateTxt_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                translateBtn_Click(this, null);
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

            this.translateTxt.Text = "";
            this.translatedTxt.Text = "";
            this.Hide();
        }

        private void OnShow(object sender, EventArgs e)
        {
            this.Show();
            this.Activate();
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
    }
}
