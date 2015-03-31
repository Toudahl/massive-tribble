using System;
using System.Collections.Generic;
using System.Text;

namespace FetchItUniversalAndApi.Models
{
    class EmailModel
    {
        private readonly string _message;
        private readonly string _subject;
        private readonly string _toAddress;

        public EmailModel(string message, string subject, string toAddress)
        {
            _message = message;
            _subject = subject;
            _toAddress = toAddress;
        }
    }
}
