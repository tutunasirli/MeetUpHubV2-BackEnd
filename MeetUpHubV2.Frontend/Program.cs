using Microsoft.AspNetCore.Authentication.Cookies;
using System;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// --- KİMLİK DOĞRULAMA VE YETKİLENDİRME ---
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(3);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
//------------------------------------------

// --- MVC VE JSON AYARLARI ---
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
//---------------------------

// --- HTTP CLIENT YAPILANDIRMASI ---
builder.Services.AddHttpClient("ApiClient", client =>
{
    // *** BACKEND API'nin DOĞRU adresini kontrol et! ***
    client.BaseAddress = new Uri("http://localhost:5292/"); // API'nin HTTP adresi
    
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});
//----------------------------------

// --- OTURUM (SESSION) AYARLARI ---
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
//------------------------------------



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Geliştirme ortamında detaylı hata sayfası
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection(); // HTTP kullandığımız için yorumlandı
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Oturum middleware

// --- KİMLİK DOĞRULAMA MIDDLEWARE ---
app.UseAuthentication(); // Cookie'yi kontrol eder
app.UseAuthorization(); // Yetkilendirme kurallarını uygular
//------------------------------------

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();