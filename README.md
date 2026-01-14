# Compact Cassette Recordings Catalog (C.C.R.C)📼

** Compact Cassette Recordings Catalog (skr. C.C.R.C.)** to intuicyjna aplikacja desktopowa stworzona dla pasjonatów analogowych nośników, służąca do zarządzania kolekcją kaset magnetofonowych. Pozwala na szczegółowe katalogowanie albumów, utworów oraz monitorowanie czasów trwania poszczególnych stron nośnika.

## 🚀 Możliwości programu

* **Zarządzanie Kolekcją:** Dodawanie, edycja i usuwanie albumów z podziałem na artystów.
* **Hierarchiczny Widok:** Przejrzyste drzewo (TreeView) grupujące albumy według artystów, z automatycznym sortowaniem alfabetycznym.
* **Szczegółowa Edycja Utworów:** Zarządzanie listą utworów z przypisaniem do strony A lub B kasety.
* **Dynamiczne Obliczanie Czasu:** Automatyczne sumowanie czasu trwania utworów dla każdej ze stron (A/B) oraz całego albumu.
* **Inteligentna Walidacja:** System zapobiegający wprowadzaniu błędnych danych (np. pustych nazw, niepoprawnych czasów trwania czy brakujących stron).
* **Responsywny Interfejs:** Dynamiczne tytuły okien informujące o aktualnie edytowanym albumie.

## 🛠️ Stos technologiczny

Projekt został zbudowany w oparciu o nowoczesne standardy platformy .NET:

* **C# / .NET:** Główny język programowania i platforma uruchomieniowa.
* **WPF (Windows Presentation Foundation):** Interfejs użytkownika zbudowany w oparciu o XAML.
* **MVVM (Model-View-ViewModel):** Architektura zapewniająca czyste oddzielenie logiki biznesowej od interfejsu graficznego.
* **FluentValidation:** Zaawansowana biblioteka do walidacji danych, zapewniająca czytelne i łatwe w utrzymaniu reguły biznesowe.
* **IDataErrorInfo:** Integracja walidacji bezpośrednio z interfejsem użytkownika WPF.

## 📦 Biblioteki i narzędzia

| Biblioteka | Przeznaczenie |
| :--- | :--- |
| **FluentValidation** | Silnik walidacji danych albumów i utworów. |
| **SQLite / Entity Framework Core** | (Dostosuj, jeśli używasz) Silnik bazy danych do przechowywania kolekcji. |

## 📸 Widok aplikacji

*(Tutaj wstawię zrzuty eranu)*

---
### Autor
**[Adrian]** Projekt rozwijany z pasją do programowania i muzyki retro.
