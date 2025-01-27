using AutoMapper;
using Application.Services;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Contracts;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthController(IUserService userService, IMapper mapper, IConfiguration configuration)
        {
            _userService = userService;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            var user = _mapper.Map<User>(registerUserDto);

            try
            {
                await _userService.RegisterUserAsync(user, registerUserDto.Password);

                // Убедимся, что пользователь был успешно сохранен
                var savedUser = await _userService.GetUserByUsernameAsync(user.Username);
                if (savedUser == null)
                {
                    throw new Exception("User registration failed.");
                }

                var tokenString = GenerateJwtToken(savedUser);
                var refreshToken = await _userService.GenerateRefreshTokenAsync(savedUser);

                Response.Cookies.Append("access_token", tokenString, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });

                Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                return Ok(new { Token = tokenString, RefreshToken = refreshToken });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userService.AuthenticateUserAsync(loginDto.Username, loginDto.Password);

            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid username or password" });
            }

            var tokenString = GenerateJwtToken(user);
            var refreshToken = await _userService.GenerateRefreshTokenAsync(user);

            Response.Cookies.Append("access_token", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new { Token = tokenString, RefreshToken = refreshToken });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            // Получаем refresh токен из cookies
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { Message = "Refresh token is missing" });
            }

            var user = await _userService.GetUserByRefreshTokenAsync(refreshToken);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid refresh token" });
            }

            var tokenString = GenerateJwtToken(user);
            var newRefreshToken = await _userService.GenerateRefreshTokenAsync(user);

            Response.Cookies.Append("access_token", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            Response.Cookies.Append("refresh_token", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new { Token = tokenString, RefreshToken = newRefreshToken });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"]!)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
