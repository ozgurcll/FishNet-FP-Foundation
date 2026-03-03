# Unity3D FishNet Modular First-Person Foundation

A robust, highly modular, and multiplayer-ready First-Person Controller foundation built for Unity3D using the **FishNet** networking solution.

## 📌 Project Overview
Most FPS controllers are tightly coupled and hard to scale, especially when transitioning from single-player to multiplayer. This project solves that by providing a **Server-Authoritative**, decoupled foundation using a **Hierarchical State Machine**. It is designed to be a clean starting point for any multiplayer FPS or interaction-based game.

## ✨ Key Features

* **Modular State Machine Architecture:** Movement logic is strictly separated into distinct states (`Idle`, `Move`, `Sprint`, `Jump`, `Air`, `Crouch`, `Dead`). This eliminates spaghetti code and massive `Update()` functions.
* **Multiplayer Ready (FishNet):** Built from the ground up for client-server architecture. Input is gathered locally, state changes are validated, and physics/health are handled authoritatively.
* **Safe Network Lifecycle Management:** Event subscriptions and inputs are safely bound to prevent memory leaks and authority desyncs during network spawn.
* **Event-Driven UI & Systems:** Health, stamina, and inventory changes are broadcasted via C# `Actions`, completely decoupling the core logic from the UI.

## 🏗️ Architecture & Core Components

The architecture relies on the **"Brain and Muscles"** pattern:
1. **`Player.cs` (The Brain):** Handles network ownership, input gathering (Unity New Input System), and State Machine transitions.
2. **`PlayerMotor.cs` (The Muscles):** Only executes physical movement (Rigidbody forces) based on the current state. Ignorant of input.
3. **`PlayerHealth` & `PlayerEquipment`:** Server-authoritative components managing damage, death states (`ObserversRpc`), and synchronized item dropping.



## 🛠️ Tech Stack
* **Engine:** Unity 2022.3+ (LTS recommended)
* **Networking:** Fish-Networking (FishNet)
* **Input:** Unity New Input System
* **Language:** C#

## 🚀 Getting Started

1. Clone this repository:
   ```bash
   git clone [https://github.com/ozgurcalli/Unity3D-Fishnet-Modular-FirstPerson-Foundation.git](https://github.com/ozgurcalli/Unity3D-Fishnet-Modular-FirstPerson-Foundation.git)



## Türkçe
   Unity3D FishNet Modüler First-Person Altyapısı
Unity3D için FishNet ağ çözümü kullanılarak geliştirilmiş; sağlam, yüksek oranda modüler ve çok oyunculuya (multiplayer) hazır bir First-Person Controller (Birinci Şahıs Kontrolcüsü) altyapısı.

📌 Proje Özeti
Çoğu FPS kontrolcüsü birbirine sıkı sıkıya bağlıdır (tightly coupled) ve özellikle tek oyunculudan çok oyunculuya geçerken ölçeklendirilmesi oldukça zordur. Bu proje, Hiyerarşik Durum Makinesi (Hierarchical State Machine) kullanarak Sunucu Yetkili (Server-Authoritative) ve izole (decoupled) bir temel sunarak bu sorunu çözer. Herhangi bir çok oyunculu FPS veya etkileşim tabanlı oyun için temiz bir başlangıç noktası olacak şekilde tasarlanmıştır.

✨ Temel Özellikler
Modüler State Machine Mimarisi: Hareket mantığı kesin sınırlarla farklı durumlara (Idle, Move, Sprint, Jump, Air, Crouch, Dead) ayrılmıştır. Bu sayede spagetti kodların ve devasa Update() fonksiyonlarının önüne geçilir.

Çok Oyunculuya Hazır (FishNet): Tamamen istemci-sunucu (client-server) mimarisine uygun olarak sıfırdan inşa edilmiştir. Girdiler (input) yerel olarak toplanır, durum değişiklikleri doğrulanır ve fizik/sağlık işlemleri sunucu yetkisiyle (authoritatively) yönetilir.

Güvenli Ağ Yaşam Döngüsü Yönetimi: Ağ üzerinde doğma (spawn) sırasında oluşabilecek bellek sızıntılarını ve yetki senkronizasyonu hatalarını önlemek için, event abonelikleri ve girdiler güvenli bir şekilde bağlanmıştır.

Event Odaklı Arayüz ve Sistemler: Sağlık, dayanıklılık (stamina) ve envanter değişiklikleri C# Action'ları aracılığıyla yayınlanır. Bu sayede temel oyun mantığı, arayüzden (UI) tamamen izole edilir.

🏗️ Mimari ve Temel Bileşenler
Sistem mimarisi "Beyin ve Kaslar" modeline dayanır:

Player.cs (Beyin): Ağ sahipliğini, girdileri (Unity New Input System) ve State Machine (Durum Makinesi) geçişlerini yönetir.

PlayerMotor.cs (Kaslar): Sadece mevcut duruma (state) bağlı olarak fiziksel hareketi (Rigidbody kuvvetleri) uygular. Girdilerden (input) habersizdir.

PlayerHealth & PlayerEquipment: Hasar, ölüm durumları (ObserversRpc) ve senkronize eşya fırlatma işlemlerini yöneten sunucu yetkili (server-authoritative) bileşenler.

🛠️ Kullanılan Teknolojiler
Oyun Motoru: Unity 2022.3+ (LTS önerilir)

Ağ Altyapısı: Fish-Networking (FishNet)

Girdi Sistemi: Unity New Input System

Dil: C#

🚀 Başlangıç
Bu repoyu bilgisayarınıza klonlayın:

Bash
git clone https://github.com/ozgurcalli/Unity3D-Fishnet-Modular-FirstPerson-Foundation.git
