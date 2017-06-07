using WindowsInput;

namespace WpfTranslator
{
    class KeyboardService
    {
        private readonly InputSimulator simulator = new InputSimulator();
        
        public void KeystrokeCtrlC(int delayMs)
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

    }
}
