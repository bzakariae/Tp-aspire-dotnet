[![Code Quality \& Linting](https://github.com/bzakariae/Tp-aspire-dotnet/actions/workflows/code-quality.yml/badge.svg)](https://github.com/bzakariae/Tp-aspire-dotnet/actions/workflows/code-quality.yml)
[![Full CI/CD \- API + Blazor](https://github.com/bzakariae/Tp-aspire-dotnet/actions/workflows/full-ci-cd.yml/badge.svg)](https://github.com/bzakariae/Tp-aspire-dotnet/actions/workflows/full-ci-cd.yml)
[![Release: v1.0](https://img.shields.io/badge/release-v1.0-blue.svg)](https://github.com/bzakariae/Tp-aspire-dotnet/releases/tag/v1.0)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/bzakariae/Tp-aspire-dotnet/blob/main/LICENSE)

<img width="1024" height="1024" alt="logo" src="https://github.com/user-attachments/assets/456ea002-c755-4922-b742-2cf72cbca169" />

<h1 align="center">FuegoCars</h1>
<p align="center">Plateforme de location de voitures de luxe pour vacances, week-ends et voyages</p>

---

## ✨ Description

FuegoCars est une application de location de voitures de luxe.  
Elle permet à des clients de réserver facilement des véhicules haut de gamme, tandis qu’un administrateur gère la flotte, les réservations et les utilisateurs.

Deux types d’utilisateurs :
- **Client** : crée un compte et réserve des voitures.
- **Administrateur** : gère les voitures, les réservations et la flotte.

> Un formulaire d’inscription est disponible dès la page d’accueil pour créer un compte client.

---

## 🏗️ Architecture & projets

Solution `.NET Aspire` composée des projets suivants :

- `MyDotNetApp.AppHost` : projet Aspire qui orchestre les services (API, front, DB, Keycloak…)
- `MyDotNetApp.ApiService` : API REST .NET pour la gestion des voitures, des réservations et des utilisateurs
- `MyDotNetApp.BlazorClient` : front-end Blazor (UI FuegoCars)
- `MyDotNetApp.ServiceDefaults` : configuration commune

---

## 🧰 Stack technique

- **Backend** : .NET / ASP.NET Core + EF Core
- **Frontend** : Blazor WebAssembly
- **Auth & IAM** : Keycloak (realm `car-rental`, clients `blazor-client` et `car-rental-admin`, rôles `ROLE_RENTAL_MANAGER` et `ROLE_RENTAL_CUSTOMER`)
- **Base de données** : PostgreSQL
- **Orchestration locale** : .NET Aspire (AppHost)

---
### 🗃️ Modèle Conceptuel de Données (MCD)
<img width="903" height="668" alt="image" src="https://github.com/user-attachments/assets/7fb6ef2f-cf43-4a54-9c61-e44c5f09dec8" />

---



## ✅ Prérequis

- [.NET SDK] (version compatible avec la solution, ex. 8.0+)
- Docker / Docker Desktop (si la DB et/ou Keycloak tournent en conteneurs)
- PostgreSQL (si utilisé en dehors de Docker)
- Keycloak (local ou via Docker), accessible sur `http://localhost:8090`
- `dotnet-ef` (pour les migrations EF Core) :

```bash
dotnet tool install --global dotnet-ef
```

---

## ⚙️ Configuration Keycloak

Le fichier d’export du realm est fourni : `realm-export.json` 

### 1. Lancer Keycloak 

```bash
docker run --name keycloak \
  -p 8090:8080 \
  -e KEYCLOAK_ADMIN=admin \
  -e KEYCLOAK_ADMIN_PASSWORD=admin \
  quay.io/keycloak/keycloak:latest start-dev
```

### 2. Importer le realm `car-rental`

1. Aller sur `http://localhost:8090/`.
2. Se connecter au **Keycloak Admin Console** (`admin` / `admin` si tu utilises l’exemple ci-dessus).
3. Dans le menu, cliquer sur **Realm selector** (en haut à gauche) → **Create Realm**.
4. Choisir **Import**, sélectionner le fichier `realm-export.json`.
5. Valider : le realm **`car-rental`** est créé avec :
   - les clients `blazor-client` (front) et `car-rental-admin`,
   - les rôles applicatifs `ROLE_RENTAL_MANAGER` et `ROLE_RENTAL_CUSTOMER`.

> Le client `blazor-client` est configuré pour rediriger vers `http://localhost:5150/*`.

---

## 🗄️ Configuration base de données

Dans la configuration de l’API (par ex. `appsettings.json` de `MyDotNetApp.ApiService` ou via variables d’environnement), renseigner la chaîne de connexion PostgreSQL, par exemple :

```json
"ConnectionStrings": {
  "mydotnetdb": "Host=localhost;Port=5432;Database=mydotnetdb;Username=postgres;Password=your_password"
}
```

Adapte le nom de connexion si nécessaire pour correspondre à ce qui est utilisé dans `Program.cs` / `builder.Configuration.GetConnectionString("mydotnetdb")`.

---

## 🧬 Migrations & initialisation de la base

Depuis le dossier racine de la solution :

1. Restaurer les dépendances :

```bash
dotnet restore
```

2. Appliquer les migrations EF Core (depuis le projet de persistance) :

```bash
cd MyDotNetApp.ApiService
dotnet ef database update
cd ..
```

> Les scripts de migration créent le schéma nécessaire pour FuegoCars (tables voitures, clients, etc.).
> Le script d’initialisation crée aussi un compte administrateur par défaut (aucune action manuelle n’est nécessaire).

---

## ▶️ Lancer l’application en local

### Option 1 — Via le projet Aspire (recommandé)

Depuis la racine du repo :

```bash
dotnet run --project MyDotNetApp.AppHost
```

Cela :
- démarre l’API (`MyDotNetApp.ApiService`),
- démarre le front Blazor (`MyDotNetApp.BlazorClient`),
- se connecte à la base PostgreSQL,
- s’intègre avec Keycloak (realm `car-rental`).

URLs typiques :
- Frontend FuegoCars (Blazor) : `http://localhost:5150`
- API : URL Aspire (ex. `http://localhost:52xx`)
- Keycloak : `http://localhost:8090`

### Option 2 — Lancer les projets séparément

1. **API** :

```bash
cd MyDotNetApp.ApiService
dotnet run
```

2. **Blazor Client** :

```bash
cd MyDotNetApp.BlazorClient
dotnet run
```

3. Vérifier que les URLs configurées dans Blazor / API pointent bien sur :
   - `Authority = http://localhost:8090/realms/car-rental`
   - `ClientId = blazor-client`

---

## 👤 Utilisation de l’application

1. **Accès à l’application**  
   Ouvrir le front Blazor dans le navigateur ( `http://localhost:5150`).

2. **Création de compte client**  
   - Sur la page d’accueil, remplir le formulaire d’inscription.
   - Une fois inscrit, se connecter pour :
     - parcourir la liste des voitures de luxe,
     - créer et gérer ses réservations.

3. **Rôles**  
   - `ROLE_RENTAL_CUSTOMER` : clients qui réservent les voitures,
   - `ROLE_RENTAL_MANAGER` : gestion de la flotte et des réservations (assignation via Keycloak).

---

## 📁 Structure du projet

```text
.
├── MyDotNetApp.sln
├── MyDotNetApp.AppHost          # Projet Aspire (orchestration)
├── MyDotNetApp.ApiService       # API REST , EF Core / migrations / accès aux données
├── MyDotNetApp.BlazorClient     # UI Blazor
├── MyDotNetApp.ServiceDefaults  # Config partagée ( healt...)
├── realm-export.json            # Export Keycloak du realm "car-rental"
└── 
```

---

## 🚀 Évolutions possibles

- Gestion avancée du pricing (saison, week-end, longue durée)
- Gestion des dommages et états des véhicules
- Intégration du paiement en ligne
- Tableau de bord admin (statistiques de réservation, Télechargement des documents en formats PDF, ...)

---
