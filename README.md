# CoreFitness

En komplett webbapplikation byggd i ASP.NET Core MVC. Projektet är utvecklat för att hantera ett gym-ekosystem där användare kan hantera medlemskap, utforska träningspass och boka klasser. Applikationen är strikt byggd enligt principerna för Clean Architecture och Domain-Driven Design (DDD) för att säkerställa en skalbar och testbar kodbas.

## Funktioner & Kravuppfyllnad

### För användaren (Medlem)
* Autentisering: Säker inloggning och registrering via ASP.NET Core Identity.
* Tredjepartsinloggning: Möjlighet att logga in snabbt via GitHub OAuth.
* Medlemskap: Teckna och hantera aktiva medlemskap kopplade till din profil.
* Bokningssystem: Se tillgängliga pass, boka platser och avboka vid behov. Systemet förhindrar automatiskt dubbelbokningar.
* Min Sida: En samlad vy för personuppgifter, bokningar och medlemskapsstatus.
* GDPR-stöd: Möjlighet för användaren att själv radera sitt konto och all associerad personlig data.

### För administratören (Admin)
* Passhantering: Behörighet att skapa nya träningspass och ta bort befintliga.
* Rollstyrning: Systemet använder rollbaserad behörighet (Admin vs Member).

### Design & UX
* Responsiv Layout: Helt anpassad för mobil, surfplatta och desktop med en funktionell hamburger-meny.
* Validering: Realtidsvalidering i frontend (JavaScript) kombinerat med robust servervalidering via ModelState.

## Teknisk Stack & Arkitektur

* Arkitektur: Clean Architecture med tydlig separation mellan Domain, Application, Infrastructure och Web.
* Mönster: Implementering av Result Pattern, Base Pattern, Unit of Work och Domain Exceptions.
* Datalagring: Entity Framework Core (Code First). Stöd för både SQLite (In-Memory för tester) och en relationsdatabas för produktion.
* Testning: Omfattande testprojekt med enhetstester för domänlogik och integrationstester med en specialiserad BaseIdentityIntegrationTest-miljö som verifierar Identity-flöden i realtid.

---
