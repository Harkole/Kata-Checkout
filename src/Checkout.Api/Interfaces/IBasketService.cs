using Checkout.Api.Models;

namespace Checkout.Api.Interfaces
{
    /// <summary>
    /// Provies domain logic for a Basket
    /// </summary>
    public interface IBasketService
    {
        /// <summary>
        /// Adds items to the basket and recalculates the cost
        /// </summary>
        /// <param name="clientId">The client identity the basket belongs too</param>
        /// <param name="basket">The basket items to add</param>
        /// <param name="cancellationToken">System cancellation object</param>
        /// <returns>The updated <c>Basket</c> details</returns>
        Task<Result<Basket>> AddItemToBasketAndCalculateCostAsync(int clientId, Basket basket, CancellationToken cancellationToken);

        /// <summary>
        /// Removes a stored client basket
        /// </summary>
        /// <param name="clientId">The client basket to permenantly delete</param>
        /// <param name="cancellationToken">System cancellation object</param>
        Task DeleteClientBasketAsync(int clientId, CancellationToken cancellationToken);

        /// <summary>
        /// Provides access to a stored client basket
        /// </summary>
        /// <param name="clientId">The client identity of the basket to load</param>
        /// <param name="cancellationToken">System cancellation object</param>
        /// <returns>The stored client basket</returns>
        Task<Result<Basket>> GetClientBasketAsync(int clientId, CancellationToken cancellationToken);

        /// <summary>
        /// Removes an item from the basket and recalculates the cost
        /// </summary>
        /// <param name="basket">The basket details to remove</param>
        /// <param name="cancellationToken">System cancellation object</param>
        /// <returns>The updated basket</returns>
        Task<Result<Basket>> RemoveItemFromBasketAndCalculateCostAsync(Basket basket, CancellationToken cancellationToken);
    }
}
