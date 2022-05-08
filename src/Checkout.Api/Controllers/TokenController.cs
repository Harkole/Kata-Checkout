using Checkout.Api.Interfaces;
using Checkout.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Checkout.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        /// <summary>
        /// Constructor, using Dependancy Injection to apply the service and Token options
        /// Both items are guarded to ensure that they are not passed null values
        /// </summary>
        /// <param name="service">The service for tokens</param>
        public TokenController(ITokenService service)
        {
            _tokenService = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Handle a new login attempt against the API, the body must contain
        /// the user details for gaining access to the system
        /// </summary>
        /// <param name="actor">The username/password model for validating the user login</param>
        /// <returns>The authenticated and signed token</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Actor actor)
        {
            object token = await _tokenService
                .GetClaimsIdentityAsync(actor, HttpContext.RequestAborted)
                .ConfigureAwait(false);

            if (null == token)
            {
                return Unauthorized();
            }

            return Ok(token);
        }

        /// <summary>
        /// Renews a valid token extending it's expire date time value
        /// NOTE: this must be an authenticated end point so that the
        /// original token is validated to prevent incorrect access
        /// </summary>
        /// <returns>The authenticated and signed token with a new expiry date time</returns>
        [HttpPost]
        public IActionResult Post()
        {
            object token = _tokenService.RenewClaimsIdentity(User);

            if (null == token)
            {
                return Unauthorized();
            }

            return Ok(token);
        }
    }
}