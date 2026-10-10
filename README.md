# MeetUpHubV2 — Etkinlik Yönetim Sistemi

.NET 8 ve ASP.NET Core Web API kullanılarak geliştirilen, katmanlı mimariye sahip bir etkinlik yönetimi uygulaması.

Proje; etkinlikler, mekanlar, odalar, kullanıcı işlemleri ve kullanıcı eşleştirme süreçlerini yönetmek üzere tasarlanmıştır. JWT tabanlı kimlik doğrulama ve SignalR ile gerçek zamanlı bildirim altyapısı içerir.

## 🚀 Kullanılan Teknolojiler

- **Backend:** C#, .NET 8, ASP.NET Core Web API
- **Authentication:** JWT Bearer Authentication, ASP.NET Core Identity
- **ORM:** Entity Framework Core 8
- **Database:** SQLite
- **Real-time Communication:** SignalR
- **API Documentation:** Swagger / Swashbuckle
- **Architecture:** Layered Architecture, Repository Pattern
- **Dependency Management:** Dependency Injection
- **Frontend Integration:** CORS, JSON Serialization

## 🏗️ Proje Mimarisi

Proje, sorumlulukları ayrı katmanlara bölerek geliştirilmiştir.

| Katman | Sorumluluk |
|---|---|
| `MeetUpHubV2.API` | API yapılandırması, controller'lar, kimlik doğrulama ve HTTP istekleri |
| `MeetUpHubV2.Business` | İş mantığı ve servis yönetimi |
| `MeetUpHubV2.DataAccess` | Veritabanı işlemleri ve repository'ler |
| `MeetUpHubV2.Entities` | Kullanıcılar, roller ve uygulama veri modelleri |
| `MeetUpHubV2.Frontend` | Kullanıcı arayüzü projesi |

## ✨ Teknik Özellikler

- JWT ile token tabanlı kimlik doğrulama yapılandırması
- ASP.NET Core Identity ile kullanıcı ve rol yönetimi
- Entity Framework Core ile veritabanı erişimi
- Repository ve service katmanlarının ayrılması
- SignalR ile gerçek zamanlı oda bildirimleri
- Swagger üzerinden API dokümantasyonu
- Dependency Injection ile servislerin yönetimi
- CORS ile frontend ve backend arasındaki iletişimin yapılandırılması
- JSON serileştirmesinde enum değerlerinin string olarak işlenmesi

## 📂 Öne Çıkan Bileşenler

- **Event:** Etkinlik işlemleri
- **Venue:** Mekan yönetimi
- **Room:** Oda yönetimi ve bildirimler
- **Matching:** Kullanıcı eşleştirme işlemleri
- **User:** Kullanıcı işlemleri
- **UserRating:** Kullanıcı değerlendirmeleri

## ⚙️ Kurulum ve Çalıştırma

### Gereksinimler

- .NET 8 SDK
- Git
- Visual Studio veya Visual Studio Code

### 1. Repository'yi klonlayın

```bash
git clone https://github.com/tutunasirli/MeetUpHubV2-BackEnd.git
cd MeetUpHubV2-BackEnd
```

### 2. Yapılandırma ayarlarını hazırlayın

API projesinin yapılandırmasında aşağıdaki ayarların bulunması gerekir:

- `ConnectionStrings:DefaultConnection`
- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:Key`

JWT anahtarını ve diğer hassas bilgileri herkese açık repository'ye yüklemeyin. Yerel geliştirme için .NET User Secrets veya uygun ortam değişkenlerini kullanın.

### 3. Bağımlılıkları yükleyin

```bash
dotnet restore
```

### 4. Projeyi derleyin

```bash
dotnet build
```

### 5. API'yi çalıştırın

```bash
dotnet run --project MeetUpHubV2.API
```

API adresi ve portu, çalışma yapılandırmasına göre değişebilir. Geliştirme ortamında Swagger arayüzü, API çalıştıktan sonra `/swagger` adresinde kullanılabilir.

> Not: Veritabanı bağlantısı, gerekli migration'lar ve yapılandırma değerleri çalıştırmadan önce kontrol edilmelidir.

## 🔌 Gerçek Zamanlı İletişim

Uygulama, oda bildirimleri için SignalR kullanır.

Hub endpoint'i:

```text
/roomhub
```

İstemcinin bağlanabilmesi için API adresinin ve kimlik doğrulama gereksinimlerinin istemci tarafında doğru yapılandırılması gerekir.

## 🔐 Güvenlik

- JWT token doğrulaması yapılandırılmıştır.
- ASP.NET Core Identity kullanıcı ve rol yönetimi için kullanılmaktadır.
- JWT anahtarları ve veritabanı bağlantı bilgileri kaynak kodunda paylaşılmamalıdır.
- Üretim ortamında HTTPS, güçlü parola politikaları ve uygun yetkilendirme kuralları kullanılmalıdır.

## 📌 Geliştirme Hedefleri

- API endpoint'lerinin ve örnek isteklerin belgelenmesi
- Birim ve entegrasyon testlerinin geliştirilmesi
- Hata yönetimi ve doğrulama süreçlerinin iyileştirilmesi
- Loglama ve izleme altyapısının geliştirilmesi
- Kurulum ve veritabanı migration adımlarının ayrıntılı belgelenmesi

---



Bu repository, yazılım geliştirme ve backend mimarisi alanındaki çalışmalarımın bir parçasıdır.
