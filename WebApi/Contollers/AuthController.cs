using Application.Abstractions;
using Application.Services;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public AuthController(IUserService userService, IMapper mapper, ITokenService tokenService)
        {
            _userService = userService;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<User>(registerUserDto);

            await _userService.RegisterUserAsync(user, registerUserDto.Password, cancellationToken);

            var savedUser = await _userService.GetUserByUsernameAsync(user.Username, cancellationToken);

            var tokenString = await _tokenService.GenerateJwtTokenAsync(savedUser, cancellationToken);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(savedUser, cancellationToken);

            SetCookies(tokenString, refreshToken);

            return Ok(new { Token = tokenString, RefreshToken = refreshToken });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
        {
            var user = await _userService.AuthenticateUserAsync(loginDto.Username, loginDto.Password, cancellationToken);

            var tokenString = await _tokenService.GenerateJwtTokenAsync(user, cancellationToken);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user, cancellationToken);

            SetCookies(tokenString, refreshToken);

            return Ok(new { Token = tokenString, RefreshToken = refreshToken });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { Message = "Refresh token is missing" });
            }

            var user = await _tokenService.GetUserByRefreshTokenAsync(refreshToken, cancellationToken);

            var tokenString = await _tokenService.GenerateJwtTokenAsync(user, cancellationToken);
            var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync(user, cancellationToken);

            SetCookies(tokenString, newRefreshToken);

            return Ok(new { Token = tokenString, RefreshToken = newRefreshToken });
        }

        private void SetCookies(string accessToken, string refreshToken)
        {
            Response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            });
        }
    }
}
