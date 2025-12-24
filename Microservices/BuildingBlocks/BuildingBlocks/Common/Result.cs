namespace BuildingBlocks.Common
{
    public class Result
    {
        public bool Success { get; }
        public ErrorResponse Error { get; }

        protected Result(bool success, ErrorResponse? error)
        {
            Success = success;
            Error = error;
        }

        public static Result Ok()
            => new Result(true, null);

        public static Result Fail(ErrorResponse error)
            => new Result(false, error);
    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        private Result(bool success, T? value, ErrorResponse? error)
            : base(success, error)
        {
            Value = value;
        }

        public static Result<T> Ok(T value)
            => new Result<T>(true, value, null);

        public static new Result<T> Fail(ErrorResponse error)
            => new Result<T>(false, default, error);
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
    }
}


