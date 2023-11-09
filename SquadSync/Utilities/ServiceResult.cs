namespace SquadSync.Utilities
{
    public class ServiceResult<T>
    {
        public T Data { get; private set; }
        public bool Success { get; private set; }
        public string ErrorMessage { get; private set; }

        private ServiceResult() { }

        public static ServiceResult<T> SuccessResult(T Data) => new ServiceResult<T> { Success = true, Data = Data };
        public static ServiceResult<T> Failure(string message) => new ServiceResult<T> { Success = false, ErrorMessage = message };
    }
}
