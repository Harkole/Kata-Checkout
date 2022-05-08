using Checkout.Api.Models;

namespace Checkout.Api.Interfaces
{
    /// <summary>
    /// All Basket repository oprerations
    /// </summary>
    public interface IBasketRepository
    {
        /// <summary>
        /// Updates the repository with the provided basket details (Create/Update)
        /// </summary>
        /// <param name="clientId">Client identity</param>
        /// <param name="basket">The basket value to store</param>
        /// <param name="cancellationToken">System cancellation object</param>
        /// <returns>The Created/Updated basket object</returns>
        Task<Result<Basket>> StoreBasketAsync(int clientId, Basket basket, CancellationToken cancellationToken);

        /// <summary>
        /// The basket to 
        /// </summary>
        /// <param name="clientId">The client identity</param>
        /// <param name="cancellationToken">System cancellation object</param>
        /// <returns>The client basket that was stored</returns>
        Task<Result<Basket>> GetBasketAsync(int clientId, CancellationToken cancellationToken);

        /// <summary>
        /// Removes the stored basket value
        /// </summary>
        /// <param name="clientId">The client identity</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A success/failure flag for deleting the basket</returns>
        Task<Result<Basket>> DeleteBasketAsync(int clientId, CancellationToken cancellationToken);
    }
}
