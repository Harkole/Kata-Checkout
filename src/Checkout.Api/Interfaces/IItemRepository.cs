using Checkout.Api.Models;

namespace Checkout.Api.Interfaces
{
    /// <summary>
    /// All item repository interactions
    /// </summary>
    public interface IItemRepository
    {
        /// <summary>
        /// Gets all items that are available to add to a basket
        /// </summary>
        /// <param name="cancellationToken">System cancellation object</param>
        /// <returns>A dictionary keyed by item name with a value of the standard cost</returns>
        Task<Result<IDictionary<string, int>>> GetItemsAsync(CancellationToken cancellationToken);
    }
}
