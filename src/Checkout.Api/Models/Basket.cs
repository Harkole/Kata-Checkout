namespace Checkout.Api.Models
{
    /// <summary>
    /// Details about a basket
    /// </summary>
    public class Basket
    {
        /// <summary>
        /// The client the basket belongs too
        /// </summary>
        public int ClientIdentity { get; set; }

        /// <summary>
        /// The items in the basket
        /// </summary>
        public IDictionary<string, int> Items { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// The total cost of the basket
        /// </summary>
        public int TotalCost { get; set; }
    }
}
