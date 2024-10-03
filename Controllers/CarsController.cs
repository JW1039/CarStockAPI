using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OracleCMS_DealerAPI.Models;
using OracleCMS_DealerAPI.Repositories;
using System;
using System.Linq;
using System.Security.Claims;

namespace OracleCMS_DealerAPI.Controllers
{
    /// <summary>
    /// Controller responsible for managing car-related operations for dealers.
    /// Handles adding, removing, searching, and listing cars for the currently logged-in dealer.
    /// </summary>
    [Authorize]
    [Route("api/cars")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly CarRepository _carRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CarsController"/> class.
        /// </summary>
        /// <param name="carRepository">Repository for managing car operations in the database.</param>
        public CarsController(CarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        /// <summary>
        /// Helper method to get the logged-in dealer's DealerId from the claims.
        /// </summary>
        /// <returns>The DealerId if available, or null if not found.</returns>
        private int? GetLoggedInDealerId()
        {
            string loggedInDealerId = User.FindFirstValue("DealerId"); // Get DealerId from claims
            if (int.TryParse(loggedInDealerId, out int dealerId))
            {
                return dealerId;
            }
            return null; // DealerId not found
        }

        /// <summary>
        /// Adds a new car to the logged-in dealer's inventory.
        /// </summary>
        /// <param name="car">The car details to be added.</param>
        /// <returns>An <see cref="IActionResult"/> indicating whether the operation was successful.</returns>
        [HttpPost]
        public IActionResult AddCar([FromBody] CarDTO car)
        {
            int? dealerId = GetLoggedInDealerId();
            if (dealerId == null)
            {
                return Unauthorized(new ErrorResponse
                {
                    StatusCode = 403,
                    Message = "You are not authorized to add cars."
                });
            }

            if (car == null)
            {
                return BadRequest(new ErrorResponse
                {
                    StatusCode = 400,
                    Message = "Car data is required."
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse
                {
                    StatusCode = 400,
                    Message = "Validation failed."
                });
            }

            try
            {

                Car insert_car = new Car
                {
                    DealerId = (int)dealerId, // DealerId comes from the logged-in user or another source
                    Make = car.Make,
                    Model = car.Model,
                    Year = car.Year,
                    NumberPlate = car.NumberPlate
                };

                _carRepository.AddCar(insert_car);
                return Ok(insert_car);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    StatusCode = 500,
                    Message = "An error occurred while adding the car: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Removes a car from the logged-in dealer's inventory by car ID.
        /// </summary>
        /// <param name="carId">The ID of the car to remove.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
        [HttpDelete("{carId}")]
        public IActionResult RemoveCar(int carId)
        {
            int? dealerId = GetLoggedInDealerId();
            if (dealerId == null)
            {
                return Unauthorized(new ErrorResponse
                {
                    StatusCode = 403,
                    Message = "You are not authorized to remove cars."
                });
            }

            try
            {
                var car = _carRepository.GetCarById(carId); // Updated to get car by its ID
                if (car == null || car.DealerId != dealerId)
                {
                    return NotFound(new ErrorResponse
                    {
                        StatusCode = 404,
                        Message = $"Car with ID {carId} not found for Dealer {dealerId}."
                    });
                }

                _carRepository.RemoveCar(carId); // Updated to remove car by its ID
                return Ok(new { message = $"Car with ID {carId} successfully removed." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    StatusCode = 500,
                    Message = "An error occurred while removing the car: " + ex.Message
                });
            }
        }


        /// <summary>
        /// Lists all cars for the logged-in dealer.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing the list of cars or an error message.</returns>
        [HttpGet]
        public IActionResult ListCars()
        {
            int? dealerId = GetLoggedInDealerId();
            if (dealerId == null)
            {
                return Unauthorized(new ErrorResponse
                {
                    StatusCode = 403,
                    Message = "You are not authorized to view cars."
                });
            }

            try
            {
                var cars = _carRepository.GetCars(dealerId.Value);
                if (cars == null || !cars.Any())
                {
                    return NotFound(new ErrorResponse
                    {
                        StatusCode = 404,
                        Message = $"No cars found for Dealer {dealerId}."
                    });
                }

                return Ok(cars);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    StatusCode = 500,
                    Message = "An error occurred while retrieving cars: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Searches for cars based on make and model for the logged-in dealer.
        /// </summary>
        /// <param name="make">The make of the car (e.g., Audi, BMW).</param>
        /// <param name="model">The model of the car (e.g., A4, X5).</param>
        /// <returns>An <see cref="IActionResult"/> containing the matching cars or an error message.</returns>
        [HttpGet("search")]
        public IActionResult SearchCars(string make, string model)
        {
            int? dealerId = GetLoggedInDealerId();
            if (dealerId == null)
            {
                return Unauthorized(new ErrorResponse
                {
                    StatusCode = 403,
                    Message = "You are not authorized to search cars."
                });
            }

            if (string.IsNullOrWhiteSpace(make) || string.IsNullOrWhiteSpace(model))
            {
                return BadRequest(new ErrorResponse
                {
                    StatusCode = 400,
                    Message = "Make and Model are required."
                });
            }

            try
            {
                var cars = _carRepository.SearchCars(dealerId.Value, make, model);
                if (cars == null || !cars.Any())
                {
                    return NotFound(new ErrorResponse
                    {
                        StatusCode = 404,
                        Message = $"No cars found for Dealer {dealerId} with Make {make} and Model {model}."
                    });
                }

                return Ok(cars);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    StatusCode = 500,
                    Message = "An error occurred while searching for cars: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Retrieves the stock levels of a specific car (make and model) for the logged-in dealer.
        /// </summary>
        /// <param name="make">The make of the car (e.g., Audi, BMW).</param>
        /// <param name="model">The model of the car (e.g., A4, X5).</param>
        /// <returns>An <see cref="IActionResult"/> containing the stock levels or an error message.</returns>
        [HttpGet("stock")]
        public IActionResult GetCarStockLevels(string make, string model)
        {
            int? dealerId = GetLoggedInDealerId();
            if (dealerId == null)
            {
                return Unauthorized(new ErrorResponse
                {
                    StatusCode = 403,
                    Message = "You are not authorized to view stock levels."
                });
            }

            if (string.IsNullOrWhiteSpace(make) || string.IsNullOrWhiteSpace(model))
            {
                return BadRequest(new ErrorResponse
                {
                    StatusCode = 400,
                    Message = "Make and Model are required."
                });
            }

            try
            {
                var stockLevels = _carRepository.GetStockLevels(dealerId.Value, make, model);
                return Ok(stockLevels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    StatusCode = 500,
                    Message = "An error occurred while retrieving stock levels: " + ex.Message
                });
            }
        }
    }
}
