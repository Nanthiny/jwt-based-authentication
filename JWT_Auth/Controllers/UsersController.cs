using JWT_Auth.Dtos;
using JWT_Auth.Interfaces;
using JWT_Auth.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWT_Auth.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserRepo _repo;
		public UsersController(IUserRepo repo)
		{
			_repo = repo;
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
						return NotFound();
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
	}
}
