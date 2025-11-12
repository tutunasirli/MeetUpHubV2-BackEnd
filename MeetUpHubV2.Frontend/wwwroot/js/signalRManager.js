// C:\Users\tutun\Documents\GitHub\MeetUpHubV2-BackEnd\MeetUpHubV2.FrontEnd\wwwroot\js\signalRManager.js

// JavaScript'te "strict mode" (katı mod) kullanmak, olası hataları engeller.
"use strict";

/**
 * Bu 'signalRManager' nesnesi, tüm SignalR bağlantı mantığımızı yönetecek.
 * _Layout.cshtml'de yüklendiği için tüm sayfalar (WaitingRoom, VotingRoom) ona erişebilecek.
 */
var signalRManager = (function () {

    // --- Değişkenler ---
    var hubConnection; // SignalR Hub bağlantımızı tutar
    
    // ÖNEMLİ: Backend API'nizin (Program.cs'de MapHub yaptığınız) adresini buraya yazın
    var hubUrl = "http://localhost:5000/roomhub"; // Backend Program.cs'deki adres

    // --- Callback Fonksiyonları ---
    // .cshtml sayfaları, "Eşleşme bulundu!" gibi bir olay olduğunda
    // ne yapılacağını bu fonksiyonları doldurarak belirleyecek.
    var onMatchFoundCallback = function () { };
    var onReceiveVenueVoteCallback = function () { };
    var onReceiveTimeVoteCallback = function () { };
    var onReceiveCurrentVotesCallback = function () { };
    var onVotingFinishedCallback = function () { };

    /**
     * Adım 1: Bağlantıyı Başlat ve Bekleme Odasına Gir (f1.png)
     * Bu metot, f1.png (Bekleme Odası) sayfası yüklendiğinde çağrılmalı.
     */
    async function startConnection() {
        if (hubConnection && hubConnection.state === "Connected") {
            console.log("SignalR zaten bağlı.");
            return;
        }

        // --- EN ÖNEMLİ BÖLÜM: YETKİLENDİRME (Authentication) ---
        // Backend'deki [Authorize] niteliğini (attribute) geçmek için
        // SignalR bağlantısına JWT token'ı eklememiz gerekir.
        // Bu token'ı, MVC (frontend) tarafında login olurken API'den alıp
        // localStorage'a kaydetmiş olman gerekiyor.
        
        // ÖNEMLİ: "jwt_token" key'ini, kendi kullandığın key ile değiştir.
        const token = localStorage.getItem("jwt_token");
        if (!token) {
            console.error("SignalR Bağlantısı: Yetkilendirme token'ı (JWT) bulunamadı. Lütfen login olduğunuzdan emin olun.");
            return;
        }
        // --------------------------------------------------------

        hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl, {
                // Token'ı bağlantıya ekliyoruz
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect() // Bağlantı koparsa otomatik tekrar dene
            .configureLogging(signalR.LogLevel.Information)
            .build();

        // --- Sunucudan Gelen Mesajları Dinle ---
        registerServerListeners();

        try {
            // Bağlantıyı başlat
            await hubConnection.start();
            console.log("SignalR bağlantısı başarılı.");

            // 1. ADIM (Client -> Server):
            // Bağlantı kurulur kurulmaz, sunucuya "Ben hazırım,
            // bekleme odasındayım" mesajını gönderiyoruz.
            // Bu, RoomHub'daki 'RegisterUserConnection' metodunu tetikler.
            await hubConnection.invoke("RegisterUserConnection");
            console.log("Kullanıcı bekleme odası için kaydedildi.");

        } catch (err) {
            console.error("SignalR bağlantı hatası: ", err);
        }
    }

    /**
     * Adım 2: Sunucudan Gelen Mesajları Dinle (f1, f3, f4)
     */
    function registerServerListeners() {
        if (!hubConnection) return;

        // --- Dinleyici 1: Eşleşme Bulundu (f.png -> f3.png tetikleyicisi) ---
        hubConnection.on("MatchFound", function (data) {
            console.log("Eşleşme bulundu!", data);
            // data objesi: { roomId: "...", options: { venues: [...], timeSlots: [...], duration: 30 } }
            
            // .cshtml sayfasının verdiği callback'i tetikle
            onMatchFoundCallback(data);
        });

        // --- Dinleyici 2: Anlık Mekan Oylarını Al (f3.png) ---
        hubConnection.on("ReceiveVenueVote", function (venueId, newVoteCount) {
            console.log(`Mekan Oyu: ${venueId}, Yeni Oy Sayısı: ${newVoteCount}`);
            onReceiveVenueVoteCallback(venueId, newVoteCount);
        });

        // --- Dinleyici 3: Anlık Saat Oylarını Al (f3.png) ---
        hubConnection.on("ReceiveTimeVote", function (timeSlot, newVoteCount) {
            console.log(`Saat Oyu: ${timeSlot}, Yeni Oy Sayısı: ${newVoteCount}`);
            onReceiveTimeVoteCallback(timeSlot, newVoteCount);
        });

        // --- Dinleyici 4: Mevcut Oyları Al (Sayfa yenilendiğinde) ---
        hubConnection.on("ReceiveCurrentVotes", function (session) {
            console.log("Odaya katıldınız, mevcut oylar alınıyor:", session);
            // session objesi: { roomId: "...", venueVotes: { "1": 2, "5": 1 }, timeVotes: { "13:00": 3 } }
            onReceiveCurrentVotesCallback(session);
        });

        // --- Dinleyici 5: Oylama Bitti (f4.png tetikleyicisi) ---
        hubConnection.on("VotingFinished", function (eventDetails) {
            console.log("Oylama bitti! Sonuç:", eventDetails);
            // eventDetails objesi: Backend'de gönderdiğimiz 'CreateEventDto'
            onVotingFinishedCallback(eventDetails);
        });
    }

    /**
     * Adım 3: Oylama Odasına Katıl (Client -> Server)
     * "MatchFound" mesajı alındıktan sonra bu çağrılmalı.
     */
    async function joinVotingRoom(roomId) {
        if (!hubConnection) return;
        try {
            await hubConnection.invoke("JoinRoomGroup", roomId);
            console.log(`Oylama odasına katıldı: ${roomId}`);
        } catch (err) {
            console.error("Oylama odasına katılınamadı: ", err);
        }
    }

    /**
     * Adım 4: Oy Gönder (f3.png) (Client -> Server)
     */
    async function castVenueVote(roomId, venueId) {
        if (!hubConnection) return;
        try {
            await hubConnection.invoke("CastVenueVote", roomId, venueId);
        } catch (err) {
            console.error("Mekan oyu gönderilemedi: ", err);
        }
    }

    async function castTimeVote(roomId, timeSlot) {
        if (!hubConnection) return;
        try {
            await hubConnection.invoke("CastTimeVote", roomId, timeSlot);
        } catch (err) {
            console.error("Zaman oyu gönderilemedi: ", err);
        }
    }
    
    // --- Dışarıya Açtığımız (Public) Fonksiyonlar ---
    // .cshtml sayfaları bu fonksiyonlara erişebilecek:
    // Örn: signalRManager.start()
    return {
        start: startConnection,
        joinRoom: joinVotingRoom,
        voteForVenue: castVenueVote,
        voteForTime: castTimeVote,

        // .cshtml sayfalarının dinleyicileri (callback) ayarlaması için
        onMatchFound: function (callback) { onMatchFoundCallback = callback; },
        onReceiveVenueVote: function (callback) { onReceiveVenueVoteCallback = callback; },
        onReceiveTimeVote: function (callback) { onReceiveTimeVoteCallback = callback; },
        onReceiveCurrentVotes: function (callback) { onReceiveCurrentVotesCallback = callback; },
        onVotingFinished: function (callback) { onVotingFinishedCallback = callback; }
    };

})(); // Bu, 'signalRManager' nesnesini hemen oluşturan bir IIFE (fonksiyon)