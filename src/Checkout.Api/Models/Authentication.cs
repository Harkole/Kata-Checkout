namespace Checkout.Api.Models
{
    /// <summary>
    /// Basic authentication details to generate a token
    /// </summary>
    public class Authentication
    {
        /// <summary>
        /// The primary contact email address
        /// </summary>
        public string? EmailAddress { get; set; }

        /// <summary>
        /// The unique system identity of the user being authenticated
        /// </summary>
        public int PrimaryId { get; set; }

        /// <summary>
        /// The secrurity group the user belongs too
        /// </summary>
        public int PrimaryGroupId { get; set; }
    }
}
