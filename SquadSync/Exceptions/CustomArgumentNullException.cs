using System.Runtime.Serialization;

namespace SquadSync.Exceptions
{
    [Serializable]
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

        protected CustomArgumentNullException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Context = info.GetString("Context");
            ParameterName = info.GetString("ParameterName");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Context", Context);
            info.AddValue("ParameterName", ParameterName);
        }
    }

}
