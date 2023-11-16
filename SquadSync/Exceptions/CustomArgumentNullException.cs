namespace SquadSync.Exceptions
{
    public class CustomArgumentNullException : ArgumentNullException
    {
        public string Context { get; }
        public string ParameterName { get; }

        public CustomArgumentNullException(string context, string paramName)
            : base($"[{context}] Argument '{paramName}' cannot be null.")
        {
            Context = context;
            ParameterName = paramName;
        }

        public CustomArgumentNullException(string context, string paramName, Exception innerException)
            : base($"[{context}] Argument '{paramName}' cannot be null.", innerException)
        {
            Context = context;
            ParameterName = paramName;
        }
    }
}
