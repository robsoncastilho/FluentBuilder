namespace Nosbor.FluentBuilder.Internals.Commands
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
