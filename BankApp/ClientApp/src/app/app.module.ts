import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { AccountComponent } from './account/account.component';
import { RegistrationComponent } from './account/registration/registration.component';
import { AccountService } from './services/account.service';
import { LoginComponent } from './account/login/login.component';
import { NavLoginMenuComponent } from './nav-menu/nav-login-menu/nav-login-menu.component';
import { JwtModule } from '@auth0/angular-jwt';

export function tokenGetter() {
  return localStorage.getItem("token");
}

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    AccountComponent,
    RegistrationComponent,
    LoginComponent,
    NavLoginMenuComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot(),
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        whitelistedDomains: ["localhost:44387"]
      }
    }),
    RouterModule.forRoot([
      {
        path: '', component: HomeComponent, pathMatch: 'full'
      },
      {
        path: 'account', component: AccountComponent,
        children: [
          {
            path: 'registration', component: RegistrationComponent
          },
          {
            path: 'login', component: LoginComponent
          },
        ]
      },
    ])
  ],
  providers: [
    AccountService,

  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
