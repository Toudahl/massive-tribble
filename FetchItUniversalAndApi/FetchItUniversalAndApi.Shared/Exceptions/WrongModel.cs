using System;
using System.Collections.Generic;
using System.Text;

namespace FetchItUniversalAndApi.Exceptions
{
    class WrongModel:Exception
    {
         public WrongModel()
        {
            
        }

        public WrongModel(string message) : base(message)
        {
            
        }

        public WrongModel(string message, Exception inner): base(message,inner)
        {
            
        }
    }
    }



