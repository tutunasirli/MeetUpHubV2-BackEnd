using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MeetUpHubV2.Frontend.Models; 
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations; 
using System; 
using System.Collections.Generic; 
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt; // Bu paket artık yüklü (CS0234 hatasını çözmüştü)
using Microsoft.Extensions.Logging; 

namespace MeetUpHubV2.Frontend.Controllers
{
    [Authorize] 
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AccountController> _logger; 

        public AccountController(IHttpClientFactory httpClientFactory, ILogger<AccountController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // --- KAYIT (REGISTER) ---
        [AllowAnonymous] 
        [HttpGet]
        public IActionResult Register()
        {
            return View(); 
        }

        [AllowAnonymous] 
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Register(RegisterViewModel model) 
        {
            if (!ModelState.IsValid)
            {
                return View(model); 
            }
            var client = _httpClientFactory.CreateClient("ApiClient");
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("api/user/register", model);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Kayıt başarıyla tamamlandı. Şimdi giriş yapabilirsiniz.";
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Kayıt başarısız: {(int)response.StatusCode} {response.ReasonPhrase} - {errorContent}");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Beklenmedik bir hata oluştu: {ex.Message}");
                return View(model);
            }
        }

        // --- GİRİŞ (LOGIN) ---
        [AllowAnonymous] 
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(); 
        }
        
        [AllowAnonymous] 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model, string? returnUrl = null)
        {
            // Bu metot artık Login.cshtml'deki JavaScript tarafından kullanılmıyor.
            ViewData["ReturnUrl"] = returnUrl;
            ModelState.AddModelError(string.Empty, "Giriş JavaScript ile yapılmalıdır.");
            return View(model);
        }

        // --- ÇIKIŞ (LOGOUT) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        
        // <<< --- DÜZELTİLMİŞ METOT (CS0117 HATASI) --- >>>
        // 'StoreTokens' satırı kaldırıldı.
        
        public class TokenDto
        {
            public string Token { get; set; }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SetAuthCookie([FromBody] TokenDto tokenDto)
        {
            if (string.IsNullOrEmpty(tokenDto.Token))
            {
                return BadRequest("Token boş olamaz.");
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(tokenDto.Token);
                
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    _logger.LogError("Token'da 'NameIdentifier' (Kullanıcı ID) claim'i bulunamadı.");
                    return StatusCode(500, "Token ID bilgisi içermiyor.");
                }

                var claims = new List<Claim>
                {
                    userIdClaim,
                    new Claim(ClaimTypes.Name, jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? ""),
                    new Claim(ClaimTypes.Email, jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "")
                };
                
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = jwtToken.ValidTo,
                    // <<< DÜZELTİLDİ: 'StoreTokens' satırı kaldırıldı (Hata veriyordu)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
                    
                _logger.LogInformation("Kullanıcı {UserId} için C# Cookie'si başarıyla oluşturuldu.", userIdClaim.Value);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SetAuthCookie sırasında hata oluştu.");
                return StatusCode(500, "Cookie oluşturulurken hata oluştu.");
            }
        }
    }
}