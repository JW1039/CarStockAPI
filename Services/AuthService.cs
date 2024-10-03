using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using OracleCMS_DealerAPI.Models;
using Dapper;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography;

namespace OracleCMS_DealerAPI.Services
{
    /// <summary>
    /// The service responsible for managing authentication-related operations for dealers.
    /// Handles login, token generation, and cookie-based authentication.
    /// </summary>
    public class AuthService
    {
        private readonly IDbConnection _dbConnection;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthService"/> class.
        /// </summary>
        /// <param name="dbConnection">The database connection to be used for authentication queries.</param>
        /// <param name="httpContextAccessor">Provides access to the current HTTP context for handling authentication cookies.</param>
        public AuthService(IDbConnection dbConnection, IHttpContextAccessor httpContextAccessor)
        {
            _dbConnection = dbConnection;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Generates an MD5 hash of the provided input string.
        /// </summary>
        /// <param name="input">The input string to hash (e.g., the dealer's password).</param>
        /// <returns>The MD5 hash as a hexadecimal string.</returns>
        public static string CreateMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return Convert.ToHexString(hashBytes);
            }
        }

        /// <summary>
        /// Generates a new authentication token (UUID) for the dealer.
        /// </summary>
        /// <returns>A unique authentication token as a string.</returns>
        private string GenerateAuthToken()
        {
            return Guid.NewGuid().ToString(); // Generate a UUID for the auth token.
        }

        /// <summary>
        /// Handles the login process for a dealer. Checks if the dealer exists in the database
        /// with valid credentials, generates an authentication token, and stores it in the Authentication table.
        /// </summary>
        /// <param name="dealer">The dealer's login details (name and password).</param>
        /// <returns>A task representing the asynchronous login operation. If successful, returns "Login successful".</returns>
        public async Task<string> LoginDealer(DealerDTO dealer)
        {
            // Hash the password before checking it against the database.
            string hashedPassword = CreateMD5(dealer.Password);

            // Query to check if the dealer exists with valid credentials.
            string dealerIdQuery = "SELECT DealerId FROM Dealers WHERE Name = @Name AND Password = @Password";
            int? dealerId = _dbConnection.QuerySingleOrDefault<int?>(dealerIdQuery, new { dealer.Name, Password = hashedPassword });

            // If the dealer was not found or credentials are invalid, return null.
            if (dealerId == null)
            {
                return null;
            }

            // Generate a new authentication token.
            string newAuthToken = GenerateAuthToken();
            DateTime tokenExpiry = DateTime.UtcNow.AddDays(7); // Token expires in 7 days.

            // Check if there's already an authentication token for this dealer.
            string tokenExistsQuery = "SELECT AuthId FROM Authentication WHERE DealerId = @DealerId";
            int? authId = _dbConnection.QuerySingleOrDefault<int?>(tokenExistsQuery, new { DealerId = dealerId });

            if (authId == null)
            {
                // If no token exists, insert a new one into the Authentication table.
                string insertTokenQuery = @"
                    INSERT INTO Authentication (DealerId, AuthToken, TokenExpiry)
                    VALUES (@DealerId, @AuthToken, @TokenExpiry)";
                _dbConnection.Execute(insertTokenQuery, new
                {
                    DealerId = dealerId,
                    AuthToken = newAuthToken,
                    TokenExpiry = tokenExpiry
                });
            }
            else
            {
                // If an authentication token exists, update it.
                string updateTokenQuery = @"
                    UPDATE Authentication
                    SET AuthToken = @AuthToken, TokenExpiry = @TokenExpiry
                    WHERE DealerId = @DealerId";
                _dbConnection.Execute(updateTokenQuery, new
                {
                    DealerId = dealerId,
                    AuthToken = newAuthToken,
                    TokenExpiry = tokenExpiry
                });
            }

            // Create the claims for the cookie.
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, dealer.Name), // Dealer's name
                new Claim("DealerId", dealerId.ToString()), // Dealer ID claim
                new Claim("AuthToken", newAuthToken) // Store the auth token in the claims
            };

            // Create the claims identity for cookie authentication.
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Keep the authentication cookie persistent.
            };

            // Sign in the user with the claims and authentication properties.
            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return "Login successful"; // Return a success message.
        }
    }
}
