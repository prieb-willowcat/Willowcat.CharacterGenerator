using System;

namespace Willowcat.CharacterGenerator.UI.Data
{

    [Serializable]
    public class ApiKeyNotFoundException : Exception
    {
        public ApiKeyNotFoundException() { }
        public ApiKeyNotFoundException(string message) : base(message) { }
        public ApiKeyNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected ApiKeyNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
