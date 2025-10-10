using BusBookingProjectApi.Models;
using BusBookingProjectApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BusBookingProjectApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 🔹 Token இல்லாவிட்டால் எல்லாம் block
    public class BusController : ControllerBase
    {
        private readonly IBusRepository _busRepo;

        public BusController(IBusRepository busRepo)
        {
            _busRepo = busRepo;
        }

        // 🔹 Admin - Add Bus
        [HttpPost("add")]
        [Authorize(Roles = "Admin")] // 🔹 Only Admin
        public async Task<IActionResult> AddBus([FromBody] Bus bus)
        {
            if (bus == null)
                return BadRequest(new { error = "Invalid bus data" });

            await _busRepo.AddBusAsync(bus);
            return Ok(new { message = "Bus added successfully" });
        }

        // 🔹 Admin - Update Bus
        [HttpPut("update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBus(int id, [FromBody] Bus bus)
        {
            var existing = await _busRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { error = "Bus not found" });

            existing.BusNumber = bus.BusNumber;
            existing.Operator = bus.Operator;
            existing.From = bus.From;
            existing.To = bus.To;
            existing.Departure = bus.Departure;
            existing.Fare = bus.Fare;
            existing.SeatsAvailable = bus.SeatsAvailable;

            await _busRepo.UpdateBusAsync(existing);
            return Ok(new { message = "Bus updated successfully" });
        }

        // 🔹 Admin - Delete Bus
        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBus(int id)
        {
            var existing = await _busRepo.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { error = "Bus not found" });

            await _busRepo.DeleteBusAsync(existing);
            return Ok(new { message = "Bus deleted successfully" });
        }

        // 🔹 User - Get All Buses
        [HttpGet("all")]
        [AllowAnonymous] // 🔹 Everyone can see bus list
        public async Task<IActionResult> GetAllBuses()
        {
            var buses = await _busRepo.GetAllBusesAsync();
            return Ok(buses);
        }

        // 🔹 Get Bus by ID
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBusById(int id)
        {
            var bus = await _busRepo.GetByIdAsync(id);
            if (bus == null)
                return NotFound(new { error = "Bus not found" });

            return Ok(bus);
        }
    }
}
