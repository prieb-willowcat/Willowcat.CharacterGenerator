using System;

namespace Willowcat.CharacterGenerator.Application.Extension
{
    public class MissingItemException : ApplicationException
    {
        public object MissingItem { get; private set; }

        public MissingItemException(string message, object missingItem)
            : base(message)
        {
            MissingItem = missingItem;
        }

        public MissingItemException(string message, Exception innerException, object missingItem)
            : base(message, innerException)
        {
            MissingItem = missingItem;
        }
    }
}
