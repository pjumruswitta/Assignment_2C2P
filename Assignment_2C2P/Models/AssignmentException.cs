namespace Assignment_2C2P.Models
{
    public class AssignmentException : Exception
    {
        public object? ExceptionData { get; set; }
        public AssignmentException(string message)
            : base(message)
        {

        }
        public AssignmentException(string message, object? exceptionData)
            : base(message)
        {
            this.ExceptionData = exceptionData;
        }
    }
}
