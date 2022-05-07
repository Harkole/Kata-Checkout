using Checkout.Api.Models;
using System.Security.Claims;

namespace Checkout.Api.Interfaces
{
    public interface ITokenService
    {
        /// <summary>
        /// Generates a new token for the provided actor
        /// </summary>
        /// <param name="actor">The Actor details to validate in the system</param>
        /// <param name="cancellationToken">System cancellation object</param>
        /// <returns>A token object containing the JWT and when it expires</returns>
        Task<object> GetclaimsIdentityAsync(Actor actor, CancellationToken cancellationToken);

        /// <summary>
        /// Generates a new token for an existing valid actor
        /// </summary>
        /// <param name="claims">The authenticated claims details</param>
        /// <returns>A token object containing the JWT and when it expires</returns>
        /// <remarks>
        /// This endpoint must be be <c>Authorized</c> to prevent faking a token and
        /// getting a new valid token out of the system
        /// </remarks>
        object RenewClaimsIdentity(ClaimsPrincipal claims);
    }
}
