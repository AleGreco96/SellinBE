using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SellinBE.Models;
using SellinBE.Models.Data;
using SellinBE.Models.Dtos;


namespace SellinBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly SellinProductionContext _context;

        public CustomersController(SellinProductionContext context)
        {
            _context = context;
        }

        // GET: api/Customers
        [HttpGet]
        private async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        private async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        private async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        private async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        private async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("GetCustomersDtoStream")]
        public async IAsyncEnumerable<CustomerDto> GetCustomersDtoStream(int skip = 0, int take = 25, string search = "")
        {
            search = search?.ToLower() ?? "";
            int? searchId = int.TryParse(search, out var id) ? id : null;

            var customersQuery = _context.Customers
                .AsNoTracking()
                .Select(c => new CustomerDto
                {
                    CustomerId = c.CustomerId,
                    Title = c.Title,
                    FirstName = c.FirstName,
                    MiddleName = c.MiddleName,
                    LastName = c.LastName,
                    Suffix = c.Suffix,
                    CompanyName = c.CompanyName,
                    EmailAddress = c.EmailAddress,
                    Phone = c.Phone,
                    CustomerAddresses = c.CustomerAddresses.Select(ca => new CustomerAddressDto
                    {
                        AddressId = ca.AddressId,
                        AddressType = ca.AddressType,
                    }).ToList(),
                })
                .Where(c => string.IsNullOrEmpty(search)
                        || c.FirstName.ToLower().Contains(search)
                        || (c.MiddleName != null && c.MiddleName.ToLower().Contains(search))
                        || c.LastName.ToLower().Contains(search)
                        || (c.Title != null && c.Title.ToLower().Contains(search))
                        || (c.Suffix != null && c.Suffix.ToLower().Contains(search))
                        || (c.CompanyName != null && c.CompanyName.ToLower().Contains(search))
                        || (c.EmailAddress != null && c.EmailAddress.ToLower().Contains(search))
                        || (searchId.HasValue && c.CustomerId == searchId.Value))
                .OrderBy(c => c.CustomerId)
                .Skip(skip)
                .Take(take)
                .AsAsyncEnumerable();

            await foreach (var customer in customersQuery)
            {
                yield return customer;
            }
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("TotalCustomersDto")]
        public int GetTotalCustomersDto(string search = "")
        {
            search = search?.ToLower() ?? "";
            int? searchId = int.TryParse(search, out var id) ? id : null;

            return _context.Customers.Where(c => string.IsNullOrEmpty(search)
                        || c.FirstName.ToLower().Contains(search)
                        || (c.MiddleName != null && c.MiddleName.ToLower().Contains(search))
                        || c.LastName.ToLower().Contains(search)
                        || (c.Title != null && c.Title.ToLower().Contains(search))
                        || (c.Suffix != null && c.Suffix.ToLower().Contains(search))
                        || (c.CompanyName != null && c.CompanyName.ToLower().Contains(search))
                        || (c.EmailAddress != null && c.EmailAddress.ToLower().Contains(search))
                        || (searchId.HasValue && c.CustomerId == searchId.Value)).Count();
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("TotalCustomers")]
        public int GetTotalCustomers()
        {
            return _context.Customers.Count();
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPut("UpdateCustomerDto")]
        public async Task<IActionResult> UpdateCustomerDto(int id, CustomerDto customerDto)
        {
            if (id != customerDto.CustomerId)
            {
                return BadRequest();
            }

            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }
            else
            {
                customer.CustomerId = customerDto.CustomerId;
                customer.Title = customerDto.Title;
                customer.FirstName = customerDto.FirstName;
                customer.MiddleName = customerDto.MiddleName;
                customer.LastName = customerDto.LastName;
                customer.CompanyName = customerDto.CompanyName;
                customer.EmailAddress = customerDto.EmailAddress;
                customer.Phone = customerDto.Phone;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
