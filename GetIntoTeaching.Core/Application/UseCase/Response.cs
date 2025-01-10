namespace GetIntoTeaching.Core.Application.UseCase
{
    /// <summary>
    /// Represents a result with a value of type <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public sealed class UseCaseResponse<TValue> : Response
    {
        private readonly TValue? _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultT{TValue}"/> class with a successful result and a value.
        /// </summary>
        /// <param name="value">The value.</param>
        private UseCaseResponse(TValue value)
        {
            _value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultT{TValue}"/> class with a failed result and an error.
        /// </summary>
        /// <param name="error">The error.</param>
        private UseCaseResponse(Error error) : base(error)
        {
            _value = default;
        }

        /// <summary>
        /// Gets the value of the result.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the result is not successful.</exception>
        public TValue Value =>
            IsSuccess ? _value! :
            throw new InvalidOperationException("Value can not be accessed when IsSuccess is false");

        /// <summary>
        /// Implicitly converts an <see cref="Error"/> to a <see cref="ResultT{TValue}"/> with a failed result.
        /// </summary>
        /// <param name="error">The error.</param>
        public static implicit operator UseCaseResponse<TValue>(Error error) =>
            new(error);

        /// <summary>
        /// Implicitly converts a value of type <typeparamref name="TValue"/> to a <see cref="ResultT{TValue}"/> with a successful result.
        /// </summary>
        /// <param name="value">The value.</param>
        public static implicit operator UseCaseResponse<TValue>(TValue value) =>
            new(value);

        /// <summary>
        /// Creates a new <see cref="ResultT{TValue}"/> with a successful result and a value.
        /// </summary>
        /// <param name="value">The value.</param>
        public static UseCaseResponse<TValue> Success(TValue value) => new(value);

        /// <summary>
        /// Creates a new <see cref="ResultT{TValue}"/> with a failed result and an error.
        /// </summary>
        /// <param name="error">The error.</param>
        public static new UseCaseResponse<TValue> Failure(Error error) => new(error);
    }

    /// <summary>
    /// Represents the result of an operation that can either be successful or contain an error.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class representing a successful result.
        /// </summary>
        protected Response()
        {
            IsSuccess = true;
            Error = default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class representing a failed result with an error.
        /// </summary>
        /// <param name="error">The error associated with the failed result.</param>
        protected Response(Error error)
        {
            IsSuccess = false;
            Error = error;
        }

        /// <summary>
        /// Gets a value indicating whether the result is successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets the error associated with the result, if any.
        /// </summary>
        public Error? Error { get; }

        /// <summary>
        /// Implicitly converts an <see cref="Error"/> to a <see cref="Result"/> representing a failed result.
        /// </summary>
        /// <param name="error">The error to convert.</param>
        /// <returns>A new instance of <see cref="Result"/> representing a failed result with the specified error.</returns>
        public static implicit operator Response(Error error) => new(error);

        /// <summary>
        /// Creates a new instance of <see cref="Result"/> representing a successful result.
        /// </summary>
        /// <returns>A new instance of <see cref="Result"/> representing a successful result.</returns>
        public static Response Success() => new();

        /// <summary>
        /// Creates a new instance of <see cref="Result"/> representing a failed result with the specified error.
        /// </summary>
        /// <param name="error">The error associated with the failed result.</param>
        /// <returns>A new instance of <see cref="Result"/> representing a failed result with the specified error.</returns>
        public static Response Failure(Error error) => new(error);
    }

    public sealed record Error(string Code, string Description)
    {
        public static readonly Error None = new(string.Empty, string.Empty);
    }
}
