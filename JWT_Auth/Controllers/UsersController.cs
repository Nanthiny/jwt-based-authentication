using JWT_Auth.Dtos;
using JWT_Auth.Interfaces;
using JWT_Auth.Models;
using JWT_Auth.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JWT_Auth.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserRepo _repo;
		private readonly TokenService _tokenService;
		public UsersController(IUserRepo repo, TokenService tokenService)
		{
			_repo = repo;
			_tokenService = tokenService;
		}
		[HttpPost("register")]
		public async Task<ActionResult> RegisterAsync([FromBody] RegisterRequest request)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var user = await _repo.RegisterAsync(request);
					return Ok(user);
				}
				else
				{
					return BadRequest();
				}
			}
			catch(Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError,ex.Message);
			}
					
		}
		[HttpPost("login")]
		public async Task<ActionResult> LoginUsersync([FromBody] LoginRequest request)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var user = await _repo.GetUserExistAsync(request);
					if (user != null)
					{
						var token = await _repo.LoginAsync(user);
						return Ok(token);
					}
					else
					{
						return Unauthorized("Invalid Credentials...");
					}
				}
				else
				{
					return BadRequest();
				}
			}catch(Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);

			}
		}
		// Refreshes an access token using a valid refresh token.
		[HttpPost("RefreshToken")]
		public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
		{
			
			try
			{
				// Validate the refresh token request.
				if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
				{
					return BadRequest("Refresh token is required.");
				}
				// Retrieve the email associated with the provided refresh token.
				var email = await _tokenService.RetrieveUserEmailByRefreshToken(request.RefreshToken);
				if (string.IsNullOrEmpty(email))
				{
					return Unauthorized("Invalid refresh token.");  // Return unauthorized if no email is found (invalid or expired token).
				}
				// Retrieve the user by email.
				var user = await _repo.GetUserByEmailExistAsync(email);
				if (user == null)
				{
					return Unauthorized("Invalid user.");  // Return unauthorized if no user is found.
				}
				// Issue a new access token and refresh token for the user.
				var accessToken = await _repo.GenerateToken(user);
				var newRefreshToken = _repo.GenerateRefreshToken();

				// Save the new refresh token.
				await _tokenService.SaveRefreshToken(user.Username, newRefreshToken);
				// Return the new access and refresh tokens.
				return Ok(new { AccessToken = accessToken, RefreshToken = newRefreshToken });
			}
			catch (Exception ex)
			{
				// Handle any exceptions during the refresh process.
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
				// Return a 500 internal server error on exception.
			}
		}
		// Revokes a refresh token to prevent its future use(after expired).
		[HttpPost("RevokeToken")]
		public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequest request)
		{
			// Validate the revocation request.
			if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
			{
				return BadRequest("Refresh token is required.");  
			}
			try
			{
				// Attempt to revoke the refresh token.
				var result = await _tokenService.RevokeRefreshToken(request.RefreshToken);
				if (!result)
				{
					return NotFound("Refresh token not found.");  
				}
				return Ok("Token revoked.");  
			}
			catch (Exception ex)
			{
				// Handle any exceptions during the revocation process.
				return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
			}
		}
	}
}
