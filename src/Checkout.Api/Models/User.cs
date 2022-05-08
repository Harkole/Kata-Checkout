namespace Checkout.Api.Models
{
    /// <summary>
    /// Details about a Client of the site
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Primary email address
        /// </summary>
        public string Email { get; set; } = default!;

        /// <summary>
        /// What the client would like to be known as
        /// </summary>
        public string Name { get; set; } = default!;

        /// <summary>
        /// The internal system identity of the client
        /// </summary>
        public int Identity { get; set; }
    }
}
