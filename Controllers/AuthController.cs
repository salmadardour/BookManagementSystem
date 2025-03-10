using BookManagementSystem.DTO; // Ensure DTO is correctly imported
using BookManagementSystem.Identity; // For ApplicationUser
using BookManagementSystem.Services; // Include TokenService
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace BookManagementSystem.Controllers
{
    /// <summary>
    /// Manages user authentication operations including registration, login, token refresh, and logout
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;

        /// <summary>
        /// Initializes a new instance of the AuthController
        /// </summary>
        /// <param name="userManager">Provides user management functionality</param>
        /// <param name="roleManager">Provides role management functionality</param>
        /// <param name="signInManager">Provides sign-in functionality</param>
        /// <param name="tokenService">Provides JWT token generation functionality</param>
        public AuthController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="registerDTO">User registration information</param>
        /// <returns>JWT token and refresh token</returns>
        /// <response code="200">Returns the new JWT token and refresh token</response>
        /// <response code="400">If the registration information is invalid</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid registration request");

            // Check if the selected role exists
            var role = registerDTO.Role ?? "User"; // Default role is "User"
            if (!await _roleManager.RoleExistsAsync(role))
                return BadRequest("Invalid role selected.");

            var user = new ApplicationUser
            {
                UserName = registerDTO.UserName,
                Email = registerDTO.Email,
                FullName = registerDTO.FullName
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            // Assign Role to the User
            await _userManager.AddToRoleAsync(user, role);

            // Generate JWT & Refresh Token
            var token = await _tokenService.GenerateTokenAsync(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return Ok(new { Token = token, RefreshToken = refreshToken });
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token
        /// </summary>
        /// <param name="loginDTO">User login credentials</param>
        /// <returns>JWT token and refresh token</returns>
        /// <response code="200">Returns the JWT token and refresh token</response>
        /// <response code="401">If credentials are invalid</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid login request");

            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDTO.Password))
                return Unauthorized("Invalid credentials");

            // Generate JWT & Refresh Token
            var token = await _tokenService.GenerateTokenAsync(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return Ok(new { Token = token, RefreshToken = refreshToken });
        }

        /// <summary>
        /// Refreshes an expired JWT token using a refresh token
        /// </summary>
        /// <param name="refreshDTO">Refresh token information</param>
        /// <returns>New JWT token and refresh token</returns>
        /// <response code="200">Returns new JWT token and refresh token</response>
        /// <response code="401">If refresh token is invalid or expired</response>
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDTO refreshDTO)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.RefreshToken == refreshDTO.RefreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return Unauthorized("Invalid or expired refresh token");

            var newToken = await _tokenService.GenerateTokenAsync(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return Ok(new { Token = newToken, RefreshToken = newRefreshToken });
        }

        /// <summary>
        /// Logs out a user by invalidating their refresh token
        /// </summary>
        /// <returns>Logout confirmation</returns>
        /// <response code="200">Returns success message</response>
        /// <response code="401">If user is not authenticated</response>
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);

            return Ok("Logged out successfully");
        }
    }
}