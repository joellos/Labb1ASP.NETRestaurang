using Labb1ASP.NETDatabas.DTOs.AdministratorDTOs;
using Labb1ASP.NETDatabas.DTOs.AuthDTOs;
using Labb1ASP.NETDatabas.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Labb1ASP.NETDatabas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IAdministratorService _administratorService;

        public AuthController(IAuthService authService, IAdministratorService administratorService)
        {
            _authService = authService;
            _administratorService = administratorService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AdministratorResponseDto>> Register([FromBody] CreateAdministratorDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var administrator = await _administratorService.CreateAdministratorAsync(request);
                return CreatedAtAction(nameof(GetAdministrator), new { id = administrator.Id }, administrator);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.LoginAsync(request);
                if (result == null)
                    return BadRequest("Invalid username or password.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.RefreshTokenAsync(request);
                if (result == null)
                    return Unauthorized("Invalid refresh token.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.RevokeTokenAsync(request);
                if (!result)
                    return BadRequest("Failed to revoke token.");

                return Ok(new { message = "Token revoked successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<AdministratorResponseDto>> GetProfile()
        {
            try
            {
                // Hämta admin ID från JWT token
                var adminIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (adminIdClaim == null || !Guid.TryParse(adminIdClaim, out var adminId))
                    return Unauthorized();

                var administrator = await _administratorService.GetAdministratorByIdAsync(adminId);
                if (administrator == null)
                    return NotFound();

                return Ok(administrator);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("test")]
        public IActionResult AuthenticatedTest()
        {
            var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            return Ok(new { message = "You are authenticated!", username });
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("admin-test")]
        public IActionResult AdminOnlyTest()
        {
            var username = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            return Ok(new { message = "You are an admin!", username });
        }

        [Authorize]
        [HttpGet("debug-claims")]
        public IActionResult DebugClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(new
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                Claims = claims,
                HasRole = User.HasClaim("role", "Administrator"),
                IsInRole = User.IsInRole("Administrator")
            });
        }

        // Helper endpoint för testing
        [HttpGet("{id}")]
        public async Task<ActionResult<AdministratorResponseDto>> GetAdministrator(Guid id)
        {
            var administrator = await _administratorService.GetAdministratorByIdAsync(id);
            if (administrator == null)
                return NotFound();

            return Ok(administrator);
        }
    }
}