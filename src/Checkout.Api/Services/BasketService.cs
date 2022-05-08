using Checkout.Api.Interfaces;
using Checkout.Api.Models;

namespace Checkout.Api.Services
{
    /// <summary>
    /// Provides all domain logice for Basket operations
    /// </summary>
    public class BasketService : IBasketService
    {
        private readonly ILogger<BasketService> _logger;
        private readonly IBasketRepository _basketRespoitory;
        private readonly IPromotionRepository _promotionRespoitory;
        private readonly IItemRepository _itemRespoitory;

        /// <summary>
        /// Service layer configuration
        /// </summary>
        /// <param name="logger">System logging object</param>
        /// <param name="basketRepository">The repository layer to use for the basket</param>
        /// <param name="promotionRepository">The repository layer to use for promotions</param>
        public BasketService(ILogger<BasketService> logger, IBasketRepository basketRepository, IPromotionRepository promotionRepository, IItemRepository itemRespoitory)
        {
            _logger = logger;
            _basketRespoitory = basketRepository;
            _promotionRespoitory = promotionRepository;
            _itemRespoitory = itemRespoitory;
        }

        /// <inheritdoc />
        public async Task<Result<Basket>> AddItemToBasketAndCalculateCostAsync(int clientId, Basket basket, CancellationToken cancellationToken)
        {
            Result<Basket> responseResult = new();

            // Get existing basket
            Result<Basket> getBasket = await _basketRespoitory
                .GetBasketAsync(clientId, cancellationToken)
                .ConfigureAwait(false);

            if (getBasket.IsSuccess)
            {
                // We've got the existing basket, merge the items in
                Basket existingBasket = getBasket.Value;

                foreach (KeyValuePair<string, int> item in basket.Items)
                {
                    if (existingBasket.Items.TryGetValue(item.Key, out int quantity))
                    {
                        // Add the new quantity to the exisitng value
                        int newQuantity = quantity + item.Value;

                        // Update the dictionary with by removing the existing details and adding the updated value
                        existingBasket.Items.Remove(item.Key);

                        if (!existingBasket.Items.TryAdd(item.Key, newQuantity))
                        {
                            // Failed to update the existing basket with new values
                            responseResult.IsSuccess = false;
                            responseResult.Message = "Failed to update items in the basket";
                            break;
                        }
                    }
                }

                if (responseResult.IsSuccess)
                {
                    // Update the basket in the repository
                    Task<Result<Basket>> storeBasketTask = _basketRespoitory.StoreBasketAsync(clientId, existingBasket, cancellationToken);

                    // Get the new basket cost value
                    Task<int> getTotalCostTask = UpdateBasketCostAsync(existingBasket, cancellationToken);

                    try
                    {
                        // Wait for both Tasks to finish
                        await Task
                            .WhenAll(storeBasketTask, getTotalCostTask)
                            .ConfigureAwait(false);

                        responseResult.IsSuccess = storeBasketTask.Result.IsSuccess;
                        responseResult.Value.TotalCost = getTotalCostTask.Result;
                    }
                    catch (AggregateException aggEx)
                    {
                        foreach (Exception ex in aggEx.InnerExceptions)
                        {
                            // Capture extra details for the exception being thrown
                            ex.Data.Add("Method", ex.TargetSite);
                            ex.Data.Add("Basket", System.Text.Json.JsonSerializer.Serialize(existingBasket));

                            // Log the error
                            _logger.LogError(ex, ex.Message);
                            _logger.LogDebug(ex.StackTrace);

                            // Update the response Result
                            responseResult.IsSuccess = false;
                            responseResult.Message = ex.Message;
                        }
                    }
                }
            }

            return responseResult;
        }

        /// <inheritdoc />
        public Task<Result<Basket>> DeleteClientBasketAsync(int clientId, CancellationToken cancellationToken)
            => _basketRespoitory.DeleteBasketAsync(clientId, cancellationToken);

        /// <inheritdoc />
        public Task<Result<Basket>> GetClientBasketAsync(int clientId, CancellationToken cancellationToken)
            => _basketRespoitory.GetBasketAsync(clientId, cancellationToken);

        /// <inheritdoc />
        public Task<Result<Basket>> RemoveItemFromBasketAndCalculateCostAsync(int clientId, Basket basket, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all promotions and then calculates the cost of the basket with the current
        /// items and quantities that are present in it
        /// </summary>
        /// <param name="basket">The basket to perform the cost calculation on</param>
        /// <param name="cancellationToken">System cancellation object</param>
        /// <returns>The updated basket cost</returns>
        private async Task<int> UpdateBasketCostAsync(Basket basket, CancellationToken cancellationToken)
        {
            int totalBasketCost = 0;

            // Create the Tasks to get the data we need
            Task<Result<IEnumerable<Promotion>>> getPromotionsTask = _promotionRespoitory.GetPromotionsAsync(cancellationToken);
            Task<Result<IDictionary<string, int>>> getItemsTask = _itemRespoitory.GetItemsAsync(cancellationToken);

            try
            {
                await Task
                        .WhenAll(getPromotionsTask, getItemsTask)
                        .ConfigureAwait(false);

                // Capture the results of the completed tasks
                Result<IEnumerable<Promotion>> promotionsResult = getPromotionsTask.Result;
                Result<IDictionary<string, int>> itemsResult = getItemsTask.Result;


                if (promotionsResult.IsSuccess && itemsResult.IsSuccess)
                {
                    // Loop over the basket
                    foreach (KeyValuePair<string, int> item in basket.Items)
                    {
                        // Get the standard item details
                        if (itemsResult.Value.TryGetValue(item.Key, out int standardCost))
                        {
                            // Look for any promotions
                            Promotion? promotion = promotionsResult.Value.FirstOrDefault(p => p.Identity == item.Key);

                            if (null != promotion)
                            {
                                // Apply promotion
                                if (item.Value >= promotion.Quantity)
                                {
                                    // The quantity in the basket is the same or larger than the promotion requirement
                                    int nonPromotionQuantity = item.Value % promotion.Quantity;
                                    int promotionQuantity = item.Value / promotion.Quantity;

                                    if (promotion.IsFixedCost)
                                    {
                                        totalBasketCost += promotionQuantity * promotion.Value;
                                    }
                                    else
                                    {
                                        int discount = ((standardCost * promotionQuantity) / 100) * promotion.Value;
                                        totalBasketCost += (standardCost * promotionQuantity) - discount;
                                    }

                                    // Add in non-promotional cost of item
                                    if (0 < nonPromotionQuantity)
                                    {
                                        totalBasketCost = nonPromotionQuantity * standardCost;
                                    }
                                }
                                else
                                {
                                    // Add promotion items that didn't meet criteria
                                    totalBasketCost += item.Value * standardCost;
                                }
                            }
                            else
                            {
                                // Add non-promotion items
                                totalBasketCost += item.Value * standardCost;

                            }
                        }
                    }
                }
            }
            catch (AggregateException aggEx)
            {
                foreach (Exception ex in aggEx.InnerExceptions)
                {
                    // Capture extra details for the exception being thrown
                    ex.Data.Add("Method", ex.TargetSite);
                    ex.Data.Add("Basket", System.Text.Json.JsonSerializer.Serialize(basket));

                    // Log the error
                    _logger.LogError(ex, ex.Message);
                    _logger.LogDebug(ex.StackTrace);
                }
            }

            return totalBasketCost;
        }
    }
}
