using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Migrations;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_VillaAPI.Repository
{
	public class UserRepository : IUserRepository
	{
		private readonly MagicVillaContext _context;
		private string secretKey;
		public UserRepository(MagicVillaContext context, IConfiguration configuration)
		{
			_context = context;
			secretKey = configuration.GetValue<string>("ApiSettings:Secret");
		}
		public bool IsUniqueUser(string username)
		{
			var user = _context.LocalUsers.FirstOrDefault(x => x.UserName == username);
			if (user == null)
			{
				return true;
			}
			return false;
		}

		public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
		{
			var user = _context.LocalUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower()
			&& u.Password == loginRequestDTO.Password);

			if (user == null)
			{
				return new LoginResponseDTO()
				{
					Token = "",
					User = null
				};
			}

			//if user was found generate JWT Token

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(secretKey);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.Name, user.Id.ToString()),
					new Claim(ClaimTypes.Role, user.Role)
				}),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
			{
				Token = tokenHandler.WriteToken(token),
				User = user
			};
			return loginResponseDTO;
		}

		public async Task<LocalUser> Register(RegistrationRequestDTO registrationRequestDTO)
		{
			LocalUser user = new()
			{
				UserName = registrationRequestDTO.UserName,
				Password = registrationRequestDTO.Password,
				Name = registrationRequestDTO.Name,
				Role = registrationRequestDTO.Role
			};
			_context.LocalUsers.Add(user);
			await _context.SaveChangesAsync();
			user.Password = "";
			return user;
		}
	}
}
