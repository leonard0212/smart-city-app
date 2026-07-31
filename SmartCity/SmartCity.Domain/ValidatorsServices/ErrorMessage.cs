namespace SmartCity.Domain.ValidatorsServices
{
    public class ErrorMessage
    {
        public string Property { get; set; }

        public string Message { get; set; }


        public ErrorMessage(string message)
        {
            Message = message;
        }

        public ErrorMessage(string message, string property)
        {
            Message = message;
            Property = property;
        }
    }
}
