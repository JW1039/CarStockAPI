using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OracleCMS_DealerAPI.Models;
using OracleCMS_DealerAPI.Services;
using System.Threading.Tasks;

namespace OracleCMS_DealerAPI.Controllers
{
    /// <summary>
    /// Controller responsible for managing dealer authentication (login, logout) and retrieving current dealer information.
    /// </summary>
    [Authorize]
    [Route("api/dealers")]
    [ApiController]
    public class DealersController : ControllerBase
    {
        private readonly AuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DealersController"/> class.
        /// </summary>
        /// <param name="authService">Service to handle authentication operations.</param>
        public DealersController(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Handles dealer login by validating the provided credentials.
        /// If successful, creates an authentication cookie for the dealer.
        /// </summary>
        /// <param name="dealer">The dealer DTO containing the login details (username and password).</param>
        /// <returns>An <see cref="IActionResult"/> indicating whether the login was successful or not.</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DealerDTO dealer)
        {
            string loginResult = await _authService.LoginDealer(dealer);
            if (loginResult == null)
            {
                return Unauthorized("Invalid credentials");
            }

            return Ok(new { message = loginResult });
        }

        /// <summary>
        /// Logs out the currently authenticated dealer by clearing the authentication cookie.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> confirming the logout.</returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Sign out the user and remove the authentication cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Logout successful" });
        }

        /// <summary>
        /// Retrieves the current authenticated dealer's username.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing the name of the logged-in dealer.</returns>
        [HttpGet("currentuser")]
        public IActionResult GetCurrentUser()
        {
            return Ok(new { User.Identity.Name });
        }
    }
}
