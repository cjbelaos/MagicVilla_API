using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
	[Route("api/UserAuth")]
	[ApiController]
	public class UserController : Controller
	{
		private readonly IUserRepository _dbUser;
		protected APIResponse _response;
		public UserController(IUserRepository dbUser)
		{
			_dbUser = dbUser;
			this._response = new();
		}
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
		{
			var loginResponse = await _dbUser.Login(model);
			if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("Username or password is incorrect");
				return BadRequest(_response);
			}
			_response.StatusCode = HttpStatusCode.OK;
			_response.IsSuccess = true;
			_response.Result = loginResponse;
			return Ok(_response);
		}
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
		{
			bool ifUserNameUnique = _dbUser.IsUniqueUser(model.Name);
			if (!ifUserNameUnique)
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("Username already exists");
				return BadRequest(_response);
			}

			var user = await _dbUser.Register(model);
			if (user == null)
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("Error while registering");
				return BadRequest(_response);
			}
			_response.StatusCode = HttpStatusCode.OK;
			_response.IsSuccess = true;
			return Ok(_response);
		}
	}
}
