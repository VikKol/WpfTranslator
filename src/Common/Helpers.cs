using System;
using System.Threading.Tasks;
using System.Windows;

namespace WpfTranslator
{
    static class Helpers
    {
        public static async Task<T> SafeExecute<T>(Func<Task<T>> func)
        {
            try
            {
                return await func();
            }
            catch (WpfTranslatorException ex)
            {
                MessageBox.Show(ex.ToString());
                return default(T);
            }
        }
    }
}
