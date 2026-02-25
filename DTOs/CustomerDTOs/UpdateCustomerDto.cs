namespace ASP_09._Swagger_documentation.DTOs.CustomerDTOs
{
    public class UpdateCustomerDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }
}
