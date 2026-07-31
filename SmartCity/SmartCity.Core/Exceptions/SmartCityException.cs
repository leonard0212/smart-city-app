namespace SmartCity.Core.Exceptions
{
    public abstract class SmartCityException : Exception
    {
        public IEnumerable<string> Messages { get; }
        protected SmartCityException(string message) : this(new[] { message }) { }

        protected SmartCityException(IEnumerable<string> messages) : base(string.Join("", messages))
        {
            Messages = messages;
        }
    }
}
