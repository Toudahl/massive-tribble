namespace FetchItUniversalAndApi.Models
{
    struct EmailModel
    {
        private string _message;
        private string _subject;
        private string _toAddress;

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }

        public string ToAddress
        {
            get { return _toAddress; }
            set { _toAddress = value; }
        }

        public EmailModel(string message, string subject, string toAddress)
        {
            _message = message;
            _subject = subject;
            _toAddress = toAddress;
        }
    }
}
