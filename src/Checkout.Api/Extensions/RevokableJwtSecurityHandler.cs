using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Checkout.Api.Extensions
{
    public class RevokableJwtSecurityHandler : JwtSecurityTokenHandler
    {
        // This needs to be something like Redis rather than in memory, ideally it doesn't want to be a database to prevent IO lag
        public static List<string> BlackList { get; } = new List<string>();

        /// <summary>
        /// Override the default ValidateToken method to include a black list check
        /// </summary>
        /// <param name="token">The token to validate</param>
        /// <param name="validationParameters">Validation Parameters</param>
        /// <param name="validatedToken">The valid token object</param>
        /// <returns>The validated ClaimsPrincipal object</returns>
        /// <remarks>
        /// To prevent extensive overhead the _blackList should be something like 
        /// Redis or other fast system with low latency. In-Memory solutions by
        /// the app are not recommended due to being lost on restart and on larger
        /// systems potentially causing a drain on system resources. Additionally
        /// don't forget to clear the JTI vlaue from the blacklist when the Token
        /// would naturally expire - again something Redis can do automatically
        /// </remarks>
        public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            // Perform the base validation to ensure we should even bother checking the revoked item
            var claimsPrincipal = base.ValidateToken(token, validationParameters, out validatedToken);

            // Get the token's unqiue identity
            var claim = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Jti);

            // Validate it
            if (claim != null && claim.ValueType == ClaimValueTypes.String)
            {
                // Check it's not black listed
                if (BlackList.Contains(claim.Value))
                {
                    // Throw an exception that it has been revoked
                    throw new SecurityTokenException("The token has been revoked");
                }
            }

            // Token is valid and not blacklisted
            return claimsPrincipal;
        }
    }
}
