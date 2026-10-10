# BankApp — Angular 22.2.2

Runtime zalecany: Node.js 24.21.0 (`.nvmrc`). Najpierw `npm ci` w tym katalogu.

## Development server

Uruchom `npm start`, następnie backend w Visual Studio w profilu Development. Otwieraj URL backendu: API obsługuje ASP.NET Core, SPA jest proxy do `http://localhost:4200`. Visual Studio nie uruchamia frontendu automatycznie. Nie używaj `--openssl-legacy-provider` ani Node 14.

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

`npm run build` tworzy produkcyjne artefakty bezpośrednio w `dist/`, zgodnie z konfiguracją hosta ASP.NET Core.

## Running unit tests

`npm test` uruchamia testy Karma/ChromeHeadless. Zainstalowany Google Chrome jest wymagany.

## Running end-to-end tests

`npm run e2e` uruchamia Playwright z Google Chrome i automatycznie uruchamia dev-server. Protractor został usunięty.

## Further help

Zachowano NgModule i istniejące trasy. Komponenty generowane przez CLI domyślnie mają `standalone: false`.

`ngx-toastr@20.0.5` deklaruje peers dla Angular 21. Wąski override dopuszcza Angular 22; renderowanie toastów sprawdza test integracyjny. Nie stosujemy globalnego `--legacy-peer-deps`. Karma ma znane podatności developerskie wymagające osobnej modernizacji test runnera.

Więcej: [dokumentacja migracji](../../docs/angular-migration.md).
