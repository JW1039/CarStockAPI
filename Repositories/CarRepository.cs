using OracleCMS_DealerAPI.Models;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OracleCMS_DealerAPI.Repositories
{
    /// <summary>
    /// Repository class responsible for managing Car entities in the database.
    /// Provides methods to add, remove, search, and retrieve cars for a specific dealer.
    /// </summary>
    public class CarRepository
    {
        private readonly IDbConnection _dbConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CarRepository"/> class.
        /// </summary>
        /// <param name="dbConnection">The database connection to be used by the repository.</param>
        public CarRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        /// <summary>
        /// Adds a new car to the database.
        /// </summary>
        /// <param name="car">The car entity to be added.</param>
        public void AddCar(Car car)
        {
            string query = "INSERT INTO Cars (Make, Model, Year, NumberPlate, DealerId) VALUES (@Make, @Model, @Year, @NumberPlate, @DealerId)";
            _dbConnection.Execute(query, car);
        }

        /// <summary>
        /// Removes a car from the database based on the dealer's ID and the car's number plate.
        /// </summary>
        /// <param name="dealerId">The ID of the dealer.</param>
        /// <param name="numberPlate">The number plate of the car to be removed.</param>
        public void RemoveCar(int carId)
        {
            string query = "DELETE FROM Cars WHERE Id = @CarId";
            _dbConnection.Execute(query, new { CarId = carId, });
        }

        /// <summary>
        /// Retrieves all cars for a specific dealer.
        /// </summary>
        /// <param name="dealerId">The ID of the dealer whose cars are to be retrieved.</param>
        /// <returns>A list of cars belonging to the specified dealer.</returns>
        public IEnumerable<Car> GetCars(int dealerId)
        {
            string query = "SELECT * FROM Cars WHERE DealerId = @DealerId";
            return _dbConnection.Query<Car>(query, new { DealerId = dealerId }).ToList();
        }


        /// <summary>
        /// Searches for cars by make and model for a specific dealer.
        /// </summary>
        /// <param name="dealerId">The ID of the dealer.</param>
        /// <param name="make">The make of the car (e.g., Audi, BMW).</param>
        /// <param name="model">The model of the car (e.g., A4, X5).</param>
        /// <returns>A list of cars that match the specified make and model for the dealer.</returns>
        public IEnumerable<Car> SearchCars(int dealerId, string make, string model)
        {
            string query = "SELECT * FROM Cars WHERE DealerId = @DealerId AND Make = @Make AND Model = @Model";
            return _dbConnection.Query<Car>(query, new { DealerId = dealerId, Make = make, Model = model }).ToList();
        }

        /// <summary>
        /// Retrieves the stock levels of a specific car (make and model) for a dealer.
        /// If no cars are found, a stock level of 0 is returned.
        /// </summary>
        /// <param name="dealerId">The ID of the dealer.</param>
        /// <param name="make">The make of the car (e.g., Audi, BMW).</param>
        /// <param name="model">The model of the car (e.g., A4, X5).</param>
        /// <returns>An object containing the make, model, and stock level of the car.</returns>
        public dynamic GetStockLevels(int dealerId, string make, string model)
        {
            string query = @"
            SELECT Make, Model, COUNT(*) AS StockLevel
            FROM Cars
            WHERE DealerId = @DealerId AND Make = @Make AND Model = @Model
            GROUP BY Make, Model";

            var result = _dbConnection.QueryFirstOrDefault<dynamic>(query, new { DealerId = dealerId, Make = make, Model = model });

            // If no result is found, return a stock level of 0
            if (result == null)
            {
                return new
                {
                    Make = make,
                    Model = model,
                    StockLevel = 0
                };
            }

            return result;
        }

        /// <summary>
        /// Retrieves a car from the database by its unique car ID.
        /// </summary>
        /// <param name="carId">The unique ID of the car to retrieve.</param>
        /// <returns>The car that matches the specified car ID, or null if not found.</returns>
        public Car GetCarById(int carId)
        {
            string query = "SELECT * FROM Cars WHERE Id = @CarId";
            return _dbConnection.QuerySingleOrDefault<Car>(query, new { CarId = carId });
        }


    }

}
