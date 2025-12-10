# Wyszukiwarka anomalii (HackNation 2025)
## Wymagania wstępne
- .NET 8, .NET10 SDK
- Visual Studio 2022 lub nowszy
- Podman Desktop

## Konfiguracja solucji
Wybierz multiple startup projects w Visual Studio dla solucji.
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
Dodac anulowanie zadań po określonym czasie oczekiwania.
### Zamiana coin flipów na prawdziwe modele
Zamienić coin flipy na prawdziwe modele.
### Anulowanie
Przeniesc rozwiązanie z sqlite na mssql.
Zainstalowac SqlTableDependency.
Dodac status.
Gdy status rozwiazania ustawiony na CANCELLED zatrzymac procesowanie workera - ustawic status STOPPED.
Dla normalnego wykonania pracy workera ustawic status COMPLETED.
### Dashboard	
Wykonac osobna aplikacje w formie dashboarda 
do wyswietlania anomalii - przekazac w URL sessionId.

https://github.com/dotnet/machinelearning/blob/main/docs/samples/Microsoft.ML.Samples/Dynamic/Trainers/AnomalyDetection/RandomizedPcaSample.cs

https://github.com/dotnet/machinelearning/blob/main/docs/samples/Microsoft.ML.Samples/Dynamic/Trainers/AnomalyDetection/RandomizedPcaSample.cs
