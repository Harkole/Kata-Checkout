namespace Checkout.Api.Models
{
    /// <summary>
    /// A system user
    /// </summary>
    public class Actor
    {
        /// <summary>
        /// The human readable identity of the user
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// The users secret to confirm their identity to the system
        /// </summary>
        public string? Password { get; set; }
    }
}
