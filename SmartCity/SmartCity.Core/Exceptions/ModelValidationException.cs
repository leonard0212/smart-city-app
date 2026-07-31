using SmartCity.Domain.ValidatorsServices;

namespace SmartCity.Core.Exceptions
{
    public class ModelValidationException : ValidationException
    {
        public ModelValidationException(ErrorMessage message) : this(new[] { message }) { }

        public ModelValidationException(IEnumerable<ErrorMessage> messages) : base(messages.Select(x => x.Message))
        {
            Messages = messages;
        }

        public new IEnumerable<ErrorMessage> Messages { get; }
    }
}
