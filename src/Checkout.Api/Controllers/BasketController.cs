using Checkout.Api.Interfaces;
using Checkout.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Checkout.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;

        /// <summary>
        /// Configuration of the Basket controller
        /// </summary>
        /// <param name="basketService">Domain logic layer to use</param>
        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        /// <summary>
        /// Addes items to the basket of the client identitfied in the route
        /// </summary>
        /// <param name="basket">The basket details to add, these should only be new items</param>
        /// <returns>The updated basket, including existing items</returns>
        [HttpPost, Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddItemToBasket(int id, [FromBody] Basket basket)
        {
            Result<Basket> result = new();

            // Only do something if we've got items to add
            if (basket.Items.Any())
            {
                result = await _basketService
                    .AddItemToBasketAndCalculateCostAsync(id, basket, HttpContext.RequestAborted)
                    .ConfigureAwait(false);
            }
            else
            {
                // There were no items to add flag an error
                result.Message = "Failed to update basket, did the request contain any items?";
                result.IsSuccess = false;
            }

            // Check if we've got a success result
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
            {
                // Something was wrong, return a BadRequest with any message value we might have
                return BadRequest(result.Message);
            }
        }

        /// <summary>
        /// Removes a basket from a specified client identity
        /// </summary>
        /// <param name="id">The client identity to remove the basket from</param>
        [HttpPost, Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteClientBasket(int id)
        {
            // Remove the basket
            Result<Basket> result = await _basketService
                .DeleteClientBasketAsync(id, HttpContext.RequestAborted)
                .ConfigureAwait(false);

            if (result.IsSuccess)
            {
                // Let the caller know we're done, this is a NoContent as it is
                // intended we'll delete the stored value before finishing the call
                return NoContent();
            }

            // Let the caller know something went wrong
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Gets a stored basket for the specific client
        /// </summary>
        /// <param name="id">The client identity</param>
        /// <returns>The stored client</returns>
        [HttpGet, Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetClientBasketAsync(int id)
        {
            Result<Basket> result = await _basketService
                .GetClientBasketAsync(id, HttpContext.RequestAborted)
                .ConfigureAwait(false);

            // If the result is successful, handle a success response
            if (result.IsSuccess)
            {
                if (!result.Value.Items.Any())
                {
                    return NoContent();
                }

                return Ok(result.Value);
            }

            // else the result was a failure, send back the error message if we have one
            return BadRequest(result.Message);
        }

        /// <summary>
        /// Removes items from the client basket
        /// </summary>
        /// <param name="id">The client identity</param>
        /// <param name="basket">The basket items to remove</param>
        /// <returns>The updated basket or <c>NoContent</c> if the basket is empty</returns>
        [HttpPost, Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveItemFromBasketAndCalculateCostAsync(int id, [FromBody] Basket basket)
        {
            Result<Basket> result = await _basketService
                .RemoveItemFromBasketAndCalculateCostAsync(id, basket, HttpContext.RequestAborted)
                .ConfigureAwait(false);

            if (result.IsSuccess)
            {
                // The operation was a success and we still have items in the Basket
                if (result.Value.Items.Any())
                {
                    return Ok(result.Value);
                }
                else
                {
                    // There's nothing in the basket so don't bother returning anything
                    return NoContent();
                }
            }

            // It's all gone wrong, let the caller know something
            return BadRequest(result.Message);
        }
    }
}
