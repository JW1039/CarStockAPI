using Microsoft.AspNetCore.Mvc;
using OracleCMS_DealerAPI.Models;

namespace OracleCMS_DealerAPI.Controllers
{
    /// <summary>
    /// Controller responsible for handling error responses.
    /// Provides custom error messages for HTTP status codes such as 404 and 403.
    /// </summary>
    [ApiController]
    [Route("error")]
    public class ErrorController : ControllerBase
    {
        /// <summary>
        /// Handles HTTP 404 (Not Found) errors.
        /// </summary>
        /// <returns>A <see cref="NotFoundObjectResult"/> with a custom error message.</returns>
        [HttpGet("404")]
        public IActionResult Handle404()
        {
            return NotFound(new ErrorResponse
            {
                StatusCode = 404,
                Message = "The resource you are looking for could not be found."
            });
        }

        /// <summary>
        /// Handles HTTP 403 (Forbidden) errors.
        /// </summary>
        /// <returns>A <see cref="ObjectResult"/> with status code 403 and a custom error message.</returns>
        [HttpGet("403")]
        public IActionResult Handle403()
        {
            return StatusCode(403, new ErrorResponse
            {
                StatusCode = 403,
                Message = "Access to the specified resource is denied."
            });
        }
    }
}
