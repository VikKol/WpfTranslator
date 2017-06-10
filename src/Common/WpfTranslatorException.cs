using System;

namespace WpfTranslator
{
    class WpfTranslatorException : ApplicationException
    {
        public WpfTranslatorException()
        {
        }

        public WpfTranslatorException(string messsage) : base(messsage)
        {
        }

        public WpfTranslatorException(string messsage, Exception innerException) : base(messsage, innerException)
        {
        }
    }
}
