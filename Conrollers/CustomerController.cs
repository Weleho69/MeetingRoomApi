using MeetingRoomApi.Data;
using MeetingRoomApi.Models;
using MeetingRoomAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MeetingRoomAPI.Conrollers
{
    [ApiController]
    [Route("api/customer")]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _context.Customers.ToArrayAsync();
            return Ok(customers);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(Customer customer)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.AddAsync(customer);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Created("created:", customer);

            }catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);  
            }
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetCustomerByEmail(string email)
        {
            var userExists = await _context.Customers.AnyAsync(c => c.Email == email);
            if(!userExists)
            {
                return NotFound("No user found by email: " + email);
            }
            var user = await _context.Customers.FirstAsync(c => c.Email == email);
            return Ok(user);
        }

        [HttpPut("{email}")]
        public async Task<IActionResult> EditUserInfo(string email, Customer updated)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var exists = await _context.Customers.AnyAsync(c => c.Email == email);
           

            try
            {
                var customer = await _context.Customers.Where(c => c.Email == email).FirstOrDefaultAsync();
                if (customer == null)
                    return NotFound();

                if (updated.Email != customer.Email)
                    customer.Email = updated.Email;
                if(updated.Name != customer.Name)
                    customer.Name = updated.Name;
                if(updated.Phone != customer.Phone)
                    customer.Phone = updated.Phone;

                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(customer);
            }catch  (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var exists = await _context.Customers.AnyAsync(c => c.Email == email);
            if (!exists)
              return NotFound(); 

            try
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
                var reservations = await _context.Reservations.Where(r => r.CustomerId == customer.Id).ToListAsync();
                _context.Reservations.RemoveRange(reservations);
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return NoContent();
            }catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }

    }
}
