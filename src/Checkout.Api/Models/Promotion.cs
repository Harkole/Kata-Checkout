namespace Checkout.Api.Models
{
    /// <summary>
    /// Details about a promotional offer
    /// </summary>
    public class Promotion
    {
        /// <summary>
        /// The identity of the product to apply the promotion too
        /// </summary>
        public string Identity { get; set; } = default!;

        /// <summary>
        /// Flag for if it's a fixed cost, if true the <c>Value</c> represents a fixed cost
        /// else if <c>false</c> then <c>Value</c> is a percentage
        /// </summary>
        public bool IsFixedCost { get; set; } = false;

        /// <summary>
        /// The quantity of the item for the promotion to take affect on
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// The percentage or actual value to use for the promotion
        /// </summary>
        public int Value { get; set; }
    }
}
