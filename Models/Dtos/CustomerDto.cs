namespace SellinBE.Models.Dtos
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string? Title { get; set; }
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = null!;
        public string? Suffix { get; set; }
        public string? CompanyName { get; set; }
        public string? EmailAddress { get; set; }
        public string? Phone { get; set; }
        public List<CustomerAddressDto>? CustomerAddresses { get; set; } 

        //public List<SalesOrderHeaderDtos>? SalesOrderHeaders { get; set; }

    }
}
