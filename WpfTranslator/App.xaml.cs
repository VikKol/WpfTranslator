using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Interop;
using Application = System.Windows.Application;

namespace WpfTranslator
{
    public partial class App : Application
    {
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;

        private readonly IntPtr wndHandle;

        private readonly Translator translator;
        private readonly SpellCheckService spellCheckService;
        private readonly KeyboardService keyboardService = new KeyboardService();

        private TranslatorWindow translatorWindow;
        private TranslatorViewModel translatorViewModel;
        private SpellCheckWindow spellCheckWindow;
        private SpellCheckViewModel spellCheckViewModel;

        public App()
        {
            InitTrayMenu();

            translator = new Translator(
                ConfigurationManager.AppSettings["MsTransaltorApiUri"],
                new StsClient(new StsSettings(ConfigurationManager.AppSettings["StsUri"], ConfigurationManager.AppSettings["SubscribtionKey"])));
            translatorViewModel = new TranslatorViewModel(translator);
            translatorWindow = new TranslatorWindow(translatorViewModel);

            spellCheckService = new SpellCheckService(ConfigurationManager.AppSettings["MashapeKey"]);
            spellCheckViewModel = new SpellCheckViewModel(spellCheckService);
            spellCheckWindow = new SpellCheckWindow(spellCheckViewModel);

            wndHandle = new WindowInteropHelper(translatorWindow).Handle;
            WinApi.RegisterAppHotKey(wndHandle);
            ComponentDispatcher.ThreadPreprocessMessage += ComponentDispatcher_ThreadPreprocessMessage;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void InitTrayMenu()
        {
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Show Translator", OnShowTranslator);
            trayMenu.MenuItems.Add("Show SpellChecker", OnShowSpellChecker);
            trayMenu.MenuItems.Add("Start with Windows", new[]
            {
                new MenuItem("Yes", (e, s) => WinApi.AddToStartup()),
                new MenuItem("No", (e, s) => WinApi.DeleteFromStartup())
            });
            trayMenu.MenuItems.Add("Exit", OnExit);
            trayIcon = new NotifyIcon
            {
                Text = WpfTranslator.Properties.Resources.AppName,
                Icon = new Icon(WpfTranslator.Properties.Resources.WpfTranslator, 40, 40),
                ContextMenu = trayMenu,
                Visible = true
            };
        }

        private void ComponentDispatcher_ThreadPreprocessMessage(ref MSG msg, ref bool handled)
        {
            if (!handled && msg.message == Constants.WmHotKey)
            {
                switch ((int)msg.wParam)
                {
                    case Constants.TranslateHotKeyId:
                    {
                        DoTranslation();
                        handled = true;
                        break;
                    }
                    case Constants.PronounceHotKeyId:
                    {
                        Speak();
                        handled = true;
                        break;
                    }
                    case Constants.SpellCheckHotKeyId:
                    {
                        CheckSpelling();
                        handled = true;
                        break;
                    }
                }
            }
        }

        private async void DoTranslation()
        {
            keyboardService.KeystrokeCtrlC(Constants.KeyPressDelayMs);
            translatorWindow.Show();
            translatorWindow.Activate();

            if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();
                await translatorViewModel.TranslateAsync(text);
            }
        }

        private async void Speak()
        {
            keyboardService.KeystrokeCtrlC(Constants.KeyPressDelayMs);

            if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();
                await translatorViewModel.PronounceAsync(text);
            }
        }

        private async void CheckSpelling()
        {
            keyboardService.KeystrokeCtrlC(Constants.KeyPressDelayMs);
            spellCheckWindow.Show();
            spellCheckWindow.Activate();

            if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();
                await spellCheckViewModel.CheckSpellingAsync(text);
            }
        }

        private void OnExit(object sender, EventArgs e)
        {
            WinApi.UnregisterAppHotKey(wndHandle);
            ComponentDispatcher.ThreadPreprocessMessage -= ComponentDispatcher_ThreadPreprocessMessage;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            trayIcon.Dispose();

            this.translatorWindow.ShouldClose = true;
            this.translatorWindow.Close();
            this.spellCheckWindow.ShouldClose = true;
            this.spellCheckWindow.Close();
            this.Shutdown();
        }
        
        private void OnShowTranslator(object sender, EventArgs e)
        {
            this.translatorWindow.Show();
            this.translatorWindow.Activate();
        }

        private void OnShowSpellChecker(object sender, EventArgs e)
        {
            this.spellCheckWindow.Show();
            this.spellCheckWindow.Activate();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString());
        }
    }
}
