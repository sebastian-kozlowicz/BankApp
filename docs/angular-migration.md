# Migracja frontendu BankApp

## Kontrakt docelowy

Najświeższa stabilna wersja potwierdzona w npm registry: Angular, CLI, Material **22.2.2**. Źródła: https://angular.dev/reference/versions, https://angular.dev/reference/releases oraz tag `latest` odpowiednich pakietów npm. Wersje prerelease nie są celem migracji.

Backend pozostaje na .NET 5; migracja frontendu nie stanowi aktualizacji bezpieczeństwa backendu.

## Uruchamianie development

1. Użyj Node.js 24.21.0 (Node 24 LTS). `.nvmrc` nie przełącza automatycznie środowiska Visual Studio.
2. W katalogu `BankApp/ClientApp` wykonaj `npm ci`, a następnie `npm start`.
3. Uruchom backend w profilu Development przez Visual Studio lub `dotnet run --project BankApp/BankApp.csproj` z katalogu repozytorium.
4. Otwórz URL backendu. Żądania API obsługuje backend; żądania SPA przekazywane są do `http://localhost:4200`.

Frontend i backend są teraz osobnymi procesami. Stary `UseAngularCliServer` parsował historyczny komunikat gotowości CLI i nie stanowi kompatybilnego kontraktu z nowoczesnym Angular CLI. Zastosowano `UseProxyToSpaDevelopmentServer`. Jeśli frontend nie jest uruchomiony, żądania SPA nie będą działać; to nie jest błąd instalacji Angulara.

## Publikacja

Target publikacji backendu uruchamia `npm ci`, potem `npm run build` (skrypt wybiera konfigurację production). Artefakty z `ClientApp/dist` są dołączane do publikacji i serwowane przez ASP.NET Core. Usunięto przestarzały argument `--prod` oraz nieaktywny target SSR (wcześniej `BuildServerSideRenderer=false`). Generowany XML dokumentacji backendu trafia do katalogu pośredniego, a stary XML w repo nie jest kopiowany — zapobiega to konfliktowi dwóch plików podczas publikacji.

## Zakres i weryfikacja

- Angular, CLI, Material i CDK 22.2.2; TypeScript 6.0.3; nowoczesny application builder zamiast Webpacka. Usunięto OpenSSL legacy provider, node-sass, Protractor i TSLint.
- Zachowano NgModule, wszystkie trasy oraz interceptor JWT poprzez `withInterceptorsFromDi()`. Utrzymano semantykę dawnych formularzy przez untyped forms; migracja na typed forms to osobny refactoring.
- Usunięto 23 wygenerowane pliki JavaScript i 23 odpowiadające mapy; źródła TypeScript pozostały.
- Build produkcyjny, 28 testów jednostkowych i 3 testy przeglądarkowe Playwright przechodzą. Testy obejmują routing, bieżący token JWT, brak wycieku tokena do zewnętrznych domen i renderowanie toastów.
- `dotnet publish` z czystym `npm ci` zakończony sukcesem; sprawdzono opublikowany `ClientApp/dist/index.html` i wskazane przez niego bundlery JS/CSS.
- Testy przeglądarkowe obejmują frontend bez realnej bazy bankowej, nie pełną transakcję finansową przez backend.
- Widok `/auth/login` ma responsywny, dostępny formularz z obsługą menedżerów haseł, stanem wysyłania i zachowaniem istniejącego przekierowania `returnUrl`.

## Ryzyka pozostające po migracji

`ngx-toastr@20.0.5` deklaruje Angular 21-only peers. Wąski override w package.json dopuszcza Angular 22, a test sprawdza faktyczne renderowanie toastów. To jawny wyjątek kompatybilności, nie oficjalna deklaracja wsparcia dostawcy.

`npm audit --omit=dev`: zero podatności. Pełny audit: sześć high findings w developerskim łańcuchu Karma/braces. Nie uruchamiać automatycznie `npm audit fix --force` — proponuje downgrade Karma. Backend nadal zgłasza koniec wsparcia .NET 5 i podatności AutoMapper/JwtBearer.

## Higiena Git

Repozytorium ignoruje `node_modules`, `.angular`, `.cache`, `.npm`, `.vite`, coverage, raporty testowe, `out-tsc`, `dist`, `dist-server` i `*.tsbuildinfo`. Reguły dla wygenerowanych `.js` i `.js.map` ograniczono do `BankApp/ClientApp/src/app`; nie ignorują plików konfiguracyjnych JavaScript.

Plik ignorowany, ale już śledzony, pozostaje śledzony. Usuwanie duplikatów wygenerowanych z TypeScript wymaga potwierdzenia odpowiednika `.ts`. `package-lock.json` pozostaje w Git jako kontrakt deterministycznej instalacji.

## Dalsze prace rozwojowe

- Usunąć wyjątek peer dependencies po wydaniu ngx-toastr wspierającego Angular 22 lub zastąpić bibliotekę.
- Zmodernizować runner testów, eliminując podatny łańcuch Karma/braces; stopniowo migrować deprecated animations.
- Zaktualizować .NET 5 i zależności backendu do wspieranej wersji; build zgłasza koniec wsparcia oraz podatności AutoMapper/JwtBearer.
- Rozszerzyć testy end-to-end o zalogowanie i operacje finansowe na izolowanej bazie testowej.
