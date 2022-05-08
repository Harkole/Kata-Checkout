namespace Checkout.Api.Models
{
    /// <summary>
    /// Provides a result object containing
    /// either the expected reulst or an error
    /// </summary>
    /// <typeparam name="T">The object type that should be returned</typeparam>
    public class Result<T> where T : class
    {
        /// <summary>
        /// Flag to show if the request was succesful or failure
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// The object value if present
        /// </summary>
        public T Value { get; set; } = default!;

        /// <summary>
        /// Addition message, expected to be an friendly/safe error message
        /// </summary>
        public string? Message { get; set; }
    }
}
