namespace SmartCity.Domain.ValidatorsServices
{
    public class ValidationResult
    {
        private readonly List<ErrorMessage> _errors;


        public ValidationResult()
        {
            _errors = new List<ErrorMessage>();
        }
        public bool Valid => !_errors.Any();

        public IEnumerable<ErrorMessage> Errors => _errors.AsReadOnly();

        public void AddError(string error)
        {
            AddError(new ErrorMessage(error));
        }

        public void AddError(ErrorMessage errorMessage)
        {
            _errors.Add(errorMessage);
        }
    }
}
