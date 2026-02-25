namespace ASP_09._Swagger_documentation.DTOs.CustomerDTOs;

public class CustomerResponseDto
{
    /// <summary>Идентификатор</summary>
    public int Id { get; set; }

    /// <summary>Имя</summary>
    public string Name { get; set; } = null!;

    /// <summary>Адрес</summary>
    public string? Address { get; set; }

    /// <summary>Email</summary>
    public string Email { get; set; } = null!;

    /// <summary>Телефон</summary>
    public string? PhoneNumber { get; set; }

    /// <summary>Дата создания</summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>Дата обновления</summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>Дата архивации (null если активен)</summary>
    public DateTimeOffset? DeletedAt { get; set; }
}
