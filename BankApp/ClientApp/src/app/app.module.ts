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
import { AuthService } from './services/auth.service';
import { NavLoginMenuComponent } from './nav-menu/nav-login-menu/nav-login-menu.component';
import { JwtModule } from '@auth0/angular-jwt';
import { NavRegistrationMenuComponent } from './nav-menu/nav-registration-menu/nav-registration-menu.component';
import { LoginComponent } from './user/login/login.component';
import { CustomerRegistrationComponent } from "./user/customer/registration/registration.component";

export function tokenGetter() {
  return sessionStorage.getItem("token");
}

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CustomerRegistrationComponent,
    LoginComponent,
    NavLoginMenuComponent,
    NavRegistrationMenuComponent,
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
        path: 'auth/registration/customer', component: CustomerRegistrationComponent
      },
      {
        path: 'auth/login', component: LoginComponent
      }
    ])
  ],
  providers: [
    AuthService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
