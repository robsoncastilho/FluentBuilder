using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nosbor.FluentBuilder.Commands
{
    internal abstract class BaseCommand
    {
        protected string ErrorMessage = "Can't set value";

        protected string AppendErrorMessage(string aditionalMessage)
        {
            return string.Format("{0} - {1}", ErrorMessage, aditionalMessage);
        }
    }
}
