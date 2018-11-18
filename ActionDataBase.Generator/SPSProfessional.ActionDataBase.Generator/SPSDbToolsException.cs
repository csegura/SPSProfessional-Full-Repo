using System;
using System.Collections.Generic;
using System.Text;

namespace SPSProfessional.ActionDataBase.Generator
{
    public class SPSDbToolsException : Exception
    {
        public SPSDbToolsException(string message) : base(message)
        {
        }

        public SPSDbToolsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
