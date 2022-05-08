using Checkout.Api.Models;

namespace Checkout.Api.Interfaces
{
    /// <summary>
    /// All promotion repository operations
    /// </summary>
    public interface IPromotionRepository
    {
        /// <summary>
        /// Gets all active promotions from the repository
        /// </summary>
        /// <param name="cancellationToken">System cancellation object</param>
        /// <returns>The collection of active promotions</returns>
        Task<Result<IEnumerable<Promotion>>> GetPromotionsAsync(CancellationToken cancellationToken);
    }
}
