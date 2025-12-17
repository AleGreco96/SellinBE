using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecurityLib.Services;
using SellinBE.Models;
using SellinBE.Models.Data;
using SellinBE.Models.Dtos;
using Credential = SecurityLib.Models.Credential;


namespace SellinBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly SellinProductionContext _context;
        private readonly DatabaseService _databaseService;
        private readonly CryptographyService _cryptographyService;

        public CustomersController(SellinProductionContext context, 
                                   DatabaseService databaseService,
                                   CryptographyService cryptography)
        {
            _context = context;
            _databaseService = databaseService;
            _cryptographyService = cryptography;
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
                .Where(c => c.IsActive)
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

            return _context.Customers.Where(c => c.IsActive).Count();
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

        [HttpPost("SignUp")]
        public async Task<ActionResult> RegisterNewCustomer(SignUpDto signUp)
        {
            // aggiungere utente anche al database della sicurezza
            // mettere in inglese gli errori di validazione nel client

            bool isEmailInDb;

            isEmailInDb = await _context.Customers.AnyAsync(c => c.EmailAddress == signUp.Email);

            if (isEmailInDb)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Email Already in Use!"
                });
            }
            else
            {
                Customer customer = new()
                {
                    Title = signUp.Title,
                    FirstName = signUp.FirstName,
                    MiddleName = signUp.MiddleName,
                    LastName = signUp.LastName,
                    CompanyName = signUp.Company,
                    EmailAddress = signUp.Email,
                    Phone = signUp.Phone,
                    IsActive = true
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                int newCustomerId = _context.Customers.Where(c => (c.EmailAddress == signUp.Email))
                                            .Select(c => c.CustomerId)
                                            .FirstOrDefault();

                if (string.IsNullOrWhiteSpace(signUp.Email))
                    throw new ArgumentNullException(nameof(signUp.Email));

                if (string.IsNullOrWhiteSpace(signUp.Password))
                    throw new ArgumentNullException(nameof(signUp.Password));

                var passwordPair = _cryptographyService.EncryptPassword(signUp.Password);

                Credential credential = new()
                {
                    CustomerId = newCustomerId,
                    EmailAddress = signUp.Email,
                    PasswordHash = passwordPair.Value,
                    PasswordSalt = passwordPair.Key,
                    Role = "User",
                    ModifiedDate = DateTime.Now
                };


                _databaseService.AddCredentialsOnDb(credential);

                return Ok(new
                {
                    Success = true,
                    Message = "SUCCESS"
                });
            }

        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}

