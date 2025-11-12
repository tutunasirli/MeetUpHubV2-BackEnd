// --- GEREKLİ USING İFADELERİ ---
using MeetUpHubV2.Entities.Dtos.RoomDtos; 
using MeetUpHubV2.Entities.Enums; 
using MeetUpHubV2.Frontend.Models;     
using Microsoft.AspNetCore.Authentication; 
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;
using System; 
using System.Net.Http; 
using System.Net.Http.Headers; 
using System.Net.Http.Json; 
using System.Security.Claims; 
using System.Threading.Tasks; 
using System.Collections.Generic; 
using System.Text.Json.Serialization; 
using System.Text.Json; 
using Microsoft.Extensions.Logging; 
using Microsoft.AspNetCore.Authentication.Cookies; 
// --- USING SONU ---

namespace MeetUpHubV2.Frontend.Controllers
{
    [Authorize] 
    public class MeetingController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MeetingController> _logger; 

        public MeetingController(IHttpClientFactory httpClientFactory, ILogger<MeetingController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger; 
        }

        // GET: /Meeting/Select?category=Coffee
        [HttpGet]
        public IActionResult Select(RoomCategory category = RoomCategory.Coffee) 
        {
            var viewModel = new SelectMeetingViewModel { Category = category };
            ViewData["CategoryName"] = category.ToString(); 
            return View(viewModel);
        }

        // POST: /Meeting/JoinMeeting
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JoinMeeting(SelectMeetingViewModel model)
        {
            _logger.LogInformation("JoinMeeting POST action started."); 

            if (!ModelState.IsValid || model.TimeSlot == null || model.Capacity == null || string.IsNullOrEmpty(model.City) || model.SelectedDate == null) 
            {
                _logger.LogWarning("JoinMeeting ModelState is invalid or selections missing."); 
                ViewData["CategoryName"] = model.Category.ToString(); 
                return View("Select", model); 
            }
            
            var accessToken = await HttpContext.GetTokenAsync("access_token"); 

            if (string.IsNullOrEmpty(accessToken))
            {
                 _logger.LogWarning("Access token not found using GetTokenAsync. User might need to re-login."); 
                ModelState.AddModelError(string.Empty, "Oturum bilgisi alınamadı veya süresi dolmuş olabilir. Lütfen tekrar giriş yapıp deneyin.");
                 ViewData["CategoryName"] = model.Category.ToString(); 
                 return View("Select", model); 
            }
             _logger.LogInformation("Access token retrieved successfully using GetTokenAsync."); 

            var client = _httpClientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", accessToken);

            var joinRequest = new MeetUpHubV2.Entities.Dtos.RoomDtos.JoinRoomRequestDto 
            {
                Category = model.Category,
                TimeSlot = model.TimeSlot.Value, 
                Capacity = model.Capacity.Value,
                SelectedDate = model.SelectedDate.Value 
            };
             _logger.LogInformation("Sending join request to API: {RequestJson}", JsonSerializer.Serialize(joinRequest)); 

            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("api/matching/join", joinRequest);
                 _logger.LogInformation("API response status code: {StatusCode}", response.StatusCode); 

                if (response.IsSuccessStatusCode) 
                {
                    string jsonContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Raw API Response JSON: {Content}", jsonContent); 

                    Models.RoomResponse? roomResponse = null; 
                    try
                    {
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        roomResponse = JsonSerializer.Deserialize<Models.RoomResponse>(jsonContent, options);
                        // === LOGLAMA DÜZELTİLDİ (CS0019 hatası için) ===
                        // Id null olabileceğinden ?? ile varsayılan değer (0) verdik
                        _logger.LogInformation("Deserialized RoomResponse: Success={Success}, RoomId={RoomId}", 
                                               roomResponse?.Success, roomResponse?.Room?.Id ?? 0); 
                        // === DÜZELTME SONU ===                                               
                    }
                    catch (JsonException jsonEx)
                    {/*... loglama ve return View ...*/} // İçerik aynı

                    if (roomResponse != null && roomResponse.Success && roomResponse.Room != null) 
                    {
                        _logger.LogInformation("Successfully joined/created room {RoomId}. Redirecting to WaitingRoom.", roomResponse.Room.Id); 
                        return RedirectToAction("WaitingRoom", new { roomId = roomResponse.Room.Id }); 
                    }
                    else if (roomResponse != null && !roomResponse.Success) 
                    {/*... loglama ve ModelState ...*/} // İçerik aynı
                    else 
                    {/*... loglama ve ModelState ...*/} // İçerik aynı
                }
                else 
                {/*... hata işleme ...*/} // İçerik aynı
            } 
            catch (HttpRequestException ex) {/*... loglama ve ModelState ...*/} // İçerik aynı
            catch (Exception ex) {/*... loglama ve ModelState ...*/} // İçerik aynı
            
            ViewData["CategoryName"] = model.Category.ToString(); 
            return View("Select", model);
        }

        // GET: /Meeting/WaitingRoom/{roomId} 
        [HttpGet]
        public async Task<IActionResult> WaitingRoom(int roomId)
        {
            // ... (Bu metot aynı kalıyor) ...
             _logger.LogInformation("WaitingRoom GET action started for RoomId: {RoomId}", roomId);
             var accessToken = await HttpContext.GetTokenAsync("access_token"); 
            if (string.IsNullOrEmpty(accessToken)) {/*... RedirectToLogin ...*/}
            var client = _httpClientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            MeetUpHubV2.Entities.Dtos.RoomDtos.RoomDto? apiRoomDto = null; 
            try {/*... API isteği ...*/} catch {/*... Hata yönetimi ve RedirectToIndex ...*/}
             if (apiRoomDto == null) {/*... Hata yönetimi ve RedirectToIndex ...*/}
            var viewModel = new WaitingRoomViewModel {/*...*/}; // Atamalar aynı
            _logger.LogInformation("Rendering WaitingRoom view for RoomId: {RoomId}", roomId); 
            return View(viewModel);
        }

        private class ErrorResponse { public string? Message { get; set; } } 

    } // Controller sonu
} // Namespace sonu