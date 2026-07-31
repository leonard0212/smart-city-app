namespace SmartCity.Core.Exceptions
{
    public class  ValidationException: SmartCityException
    {
        public ValidationException(string message) : base(message) { }
        public ValidationException(IEnumerable<string> messages) : base(messages) { }
    }
}
