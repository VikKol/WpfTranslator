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

            spellCheckViewModel = new SpellCheckViewModel();
            spellCheckWindow = new SpellCheckWindow(spellCheckViewModel);

            wndHandle = new WindowInteropHelper(translatorWindow).Handle;
            WinApi.RegisterAppHotKey(wndHandle);
            ComponentDispatcher.ThreadPreprocessMessage += ComponentDispatcher_ThreadPreprocessMessage;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void InitTrayMenu()
        {
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Show Translator", ShowTranslator);
            trayMenu.MenuItems.Add("Show SpellChecker", ShowSpellChecker);

            var yesMi = new MenuItem("Yes") { Checked = WinApi.IsAddedToStartup(), RadioCheck = true };
            var noMi = new MenuItem("No") { Checked = !WinApi.IsAddedToStartup(), RadioCheck = true };
            yesMi.Click += (s, e) =>
            {
                WinApi.AddToStartup();
                yesMi.Checked = true;
                noMi.Checked = false;
            };
            noMi.Click += (s, e) =>
            {
                WinApi.AddToStartup();
                yesMi.Checked = false;
                noMi.Checked = true;
            };

            trayMenu.MenuItems.Add("Start with Windows", new[] { yesMi, noMi });
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
                        DoTranslationAsync();
                        handled = true;
                        break;
                    }
                    case Constants.PronounceHotKeyId:
                    {
                        SpeakAsync();
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

        private async void DoTranslationAsync()
        {
            keyboardService.KeystrokeCtrlC(Constants.KeyPressDelayMs);
            ShowTranslator();

            if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();
                await translatorViewModel.TranslateAsync(text);
            }
        }

        private async void SpeakAsync()
        {
            keyboardService.KeystrokeCtrlC(Constants.KeyPressDelayMs);

            if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();
                await translatorViewModel.PronounceAsync(text);
            }
        }

        private void CheckSpelling()
        {
            keyboardService.KeystrokeCtrlC(Constants.KeyPressDelayMs);
            ShowSpellChecker();

            if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();
                spellCheckViewModel.EnglishText = text;
                spellCheckViewModel.RussianText = text;
                spellCheckViewModel.UkrainianText = text;
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
        
        private void ShowTranslator(object sender = null, EventArgs e = null)
        {
            this.translatorWindow.Show();
            this.translatorWindow.Activate();
        }

        private void ShowSpellChecker(object sender = null, EventArgs e = null)
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
