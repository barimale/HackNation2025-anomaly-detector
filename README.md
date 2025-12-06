# Wyszukiwarka anomalii (HackNation 2025)
## Wymagania wstępne
- .NET 8, .NET10 SDK
- Visual Studio 2022 lub nowszy
- Podman Desktop

## Konfiguracja środowiska
Stwórz szkielet katalogów z wykorzystaniem testu jednostkowego 'Create_folders()'.

Stwórz modele ML.NET z wykorzystaniem testu jednostkowego 'Create_ML_models()'.
## Uruchomienie - warunki wstępne
Utwórz kontenery zgodnie z definicjami umieszczonymi w PODMAN.md.
## Uruchomienie aplikacji
Włącz frontend aplikacji poprzez wydanie komendy yarn start.
Włącz backend aplikacji poprzez wybranie przycisku START w Visual Studio.
Upewnij się, że dane początkowe są załadowane do bazy danych SQLite.

## Oczekiwane rezultaty
Aplikacja powinna uruchomić się poprawnie i być dostępna pod adresem http://localhost:8081.
W ramach backendu powinno się uruchomić 5 workerów oraz jeden serwer API pod adresem http://localhost:53654.
Baza danych powinna zostać wypełniona danymi początkowymi.

## Opcjonalne funkcje aplikacyjne do wykonania

https://ai.google.dev/gemini-api/docs/models#experimental

https://blog.elmah.io/find-anomalies-with-spike-detection-and-ml-net/

https://github.com/IsNemoEqualTrue/monitor-table-change-with-sqltabledependency
https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-10.0&tabs=visual-studio

https://libraries.io/nuget/SqlTableDependency
