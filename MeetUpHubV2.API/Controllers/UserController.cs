using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MeetUpHubV2.Entities;
using MeetUpHubV2.Entities.Dtos.UserDtos; // Bu satırın eklendiğinden emin ol
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MeetUpHubV2.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public UserController(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // Kullanıcı adı (Username) yerine Email ile kontrol ediyoruz
            var userExists = await _userManager.FindByEmailAsync(registerDto.Email);
            if (userExists != null)
                return BadRequest("Bu email adresi zaten kullanılıyor!");

            User newUser = new()
            {
                // Identity'nin zorunlu UserName alanı için Email'i kullanıyoruz
                Email = registerDto.Email,
                UserName = registerDto.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                
                // RegisterDto'ndaki diğer tüm alanlar
               // TcNo = registerDto.TcNo,
                Name = registerDto.Name,
                Surname = registerDto.Surname,
                BirthDate = registerDto.BirthDate,
                PhoneNumber = registerDto.PhoneNumber,
                RegistrationDate = DateTime.Now,
                AccountStatus = "Beklemede"
                
            };

            var result = await _userManager.CreateAsync(newUser, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Kullanıcı başarıyla oluşturuldu!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Kullanıcıyı 'Username' ile değil, 'Email' ile arıyoruz
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            // Şifreyi kontrol ediyoruz
            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName), // Token'a kullanıcı adı olarak (ki bu email) ekle
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                
                var token = GenerateJwtToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            // Hata mesajını düzeltiyoruz
            return Unauthorized("Email veya şifre hatalı.");
        }
        
        private JwtSecurityToken GenerateJwtToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
    }
}