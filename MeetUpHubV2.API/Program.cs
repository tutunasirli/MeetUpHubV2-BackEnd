using System.Text; // JWT için eklendi
using MeetUpHubV2.API.SignalR; // Hub'ı buraya taşıdığımız için namespace'i kontrol et
using MeetUpHubV2.Business.Abstract;
using MeetUpHubV2.Business.Concrete;
using MeetUpHubV2.DataAccess; // DbContext için eklendi
using MeetUpHubV2.DataAccess.Abstract;
using MeetUpHubV2.DataAccess.Concrete;
using MeetUpHubV2.Entities; // User ve Role için eklendi
using Microsoft.AspNetCore.Authentication.JwtBearer; // JWT için eklendi
using Microsoft.AspNetCore.Identity; // Identity için eklendi
using Microsoft.EntityFrameworkCore; // DbContext için eklendi
using Microsoft.IdentityModel.Tokens; // JWT için eklendi
using MeetUpHubV2.API.Services;
using System.Text.Json.Serialization; // 🔥 Enum converter için eklendi

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration; // appsettings.json'a erişim için eklendi

// CORS Policy – frontend portuna göre
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5000") // frontend portu
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// --- JSON AYARLARI ---
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Döngüsel referansları yoksay
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;

        // 🔥 Enum'ları string olarak parse etme desteği
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- IDENTITY VE VERİTABANI BÖLÜMÜ ---

// 1. Veritabanı Bağlantısı
builder.Services.AddDbContext<MeetUpHubV2DbContext>(options =>
{
    options.UseSqlite(config.GetConnectionString("DefaultConnection"));
});

// 2. Identity Servisleri
builder.Services.AddIdentity<User, Role>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<MeetUpHubV2DbContext>()
.AddDefaultTokenProviders();

// 3. JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
        config["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key appsettings.json dosyasında bulunamadı!")
    ))
    };
});

// --- SERVİSLERİN KAYDI ---
builder.Services.AddScoped<IRoomNotificationService, SignalRNotificationService>();

builder.Services.AddScoped<IRoomService, RoomManager>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();

builder.Services.AddScoped<IVenueService, VenueManager>();
builder.Services.AddScoped<IVenueRepository, VenueRepository>();

builder.Services.AddScoped<IEventService, EventManager>();
builder.Services.AddScoped<IEventRepository, EventRepository>();

builder.Services.AddScoped<IMatchingService, MatchingManager>();

builder.Services.AddScoped<IUserService, UserManager>();


builder.Services.AddScoped<IUserRatingRepository, UserRatingRepository>();
builder.Services.AddScoped<IUserRatingService, UserRatingManager>();


// SignalR + In-Memory Cache
builder.Services.AddSignalR();
builder.Services.AddMemoryCache();

var app = builder.Build();

// --- MIDDLEWARE PIPELINE ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<RoomHub>("/roomhub");

app.Run();
