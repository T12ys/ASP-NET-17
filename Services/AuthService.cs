using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ASP_09._Swagger_documentation.Data;
using ASP_09._Swagger_documentation.DTOs.AuthDTOs;
using ASP_09._Swagger_documentation.Models;
using ASP_09._Swagger_documentation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ASP_09._Swagger_documentation.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _cfg;

    public AuthService(AppDbContext context, IConfiguration cfg)
    {
        _context = context;
        _cfg = cfg;
    }

    // ---------------- REGISTER ----------------
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var exists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
        if (exists)
            throw new InvalidOperationException("Пользователь с таким Email уже существует.");

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Address = dto.Address,
            PhoneNumber = dto.PhoneNumber,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return await IssueTokensAsync(user);
    }

    // ---------------- LOGIN ----------------
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Неверный Email или пароль.");

        return await IssueTokensAsync(user);
    }

    // ---------------- REFRESH ----------------
    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
            throw new UnauthorizedAccessException("Refresh token отсутствует.");

        var hash = HashToken(refreshToken);

        var existing = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == hash);

        if (existing is null || existing.RevokedAt is not null || existing.ExpiresAt <= DateTimeOffset.UtcNow)
            throw new UnauthorizedAccessException("Недействительный refresh token.");

        // Ревокируем старый токен
        existing.RevokedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();

        return await IssueTokensAsync(existing.User);
    }

    // ---------------- LOGOUT ----------------
    public async Task LogoutAsync(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken)) return;

        var hash = HashToken(refreshToken);
        var existing = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == hash);
        if (existing is null) return;

        existing.RevokedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();
    }

    // ---------------- UPDATE PROFILE ----------------
    public async Task<AuthResponseDto> UpdateProfileAsync(int userId, UpdateProfileDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
            throw new KeyNotFoundException("Пользователь не найден.");

        user.Name = dto.Name;
        user.Address = dto.Address;
        user.PhoneNumber = dto.PhoneNumber;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();

        var access = GenerateAccessToken(user, out var expiresAt);

        return new AuthResponseDto
        {
            AccessToken = access,
            AccessTokenExpiresAt = expiresAt,
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name
        };
    }

    // ---------------- CHANGE PASSWORD ----------------
    public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
            throw new KeyNotFoundException("Пользователь не найден.");

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Текущий пароль неверен.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.UpdatedAt = DateTimeOffset.UtcNow;

        // Ревокируем все refresh токены — после смены пароля нужно войти заново
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync();

        foreach (var token in tokens)
            token.RevokedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();
    }

    // ---------------- HELPERS ----------------
    private async Task<AuthResponseDto> IssueTokensAsync(User user)
    {
        var access = GenerateAccessToken(user, out var expiresAt);
        var refresh = CreateRefreshTokenString();

        _context.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = HashToken(refresh),
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(GetRefreshTokenDays()),
            CreatedAt = DateTimeOffset.UtcNow
        });

        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = access,
            AccessTokenExpiresAt = expiresAt,
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            RefreshToken = refresh
        };
    }

    private string GenerateAccessToken(User user, out DateTime expiresAt)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var minutes = int.Parse(_cfg["Jwt:AccessTokenMinutes"] ?? "15");
        expiresAt = DateTime.UtcNow.AddMinutes(minutes);

        var token = new JwtSecurityToken(
            issuer: _cfg["Jwt:Issuer"],
            audience: _cfg["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string CreateRefreshTokenString()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    private static string HashToken(string token)
    {
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hash);
    }

    private int GetRefreshTokenDays() =>
        int.Parse(_cfg["Jwt:RefreshTokenDays"] ?? "7");
}