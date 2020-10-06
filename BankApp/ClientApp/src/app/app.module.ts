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
import { NavCustomerMenuComponent } from './nav-menu/nav-customer-menu/nav-customer-menu.component';
import { AuthGuard } from "./services/auth-guard.service";
import { AdminAuthGuard } from "./services/admin-auth-guard.service";
import { NoAccessComponent } from './user/no-access/no-access.component';
import { AdminPanelComponent } from './user/administrator/admin-panel/admin-panel.component';
import { NavAdministratorMenuComponent } from './nav-menu/nav-administrator-menu/nav-administrator-menu.component';
import { MaterialModule } from './material/material.module';
import { BankAccountSummaryComponent } from './user/customer/bank-account-summary/bank-account-summary.component';
import { CustomerProfileComponent } from './user/customer/customer-profile/customer-profile.component';
import { BankTransferComponent } from './user/customer/bank-transfer/bank-transfer.component';
import { NewBranchComponent } from './branch/new-branch/new-branch.component';
import { AddressFormComponent } from "./address/address-form/address-form.component";

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
    NavCustomerMenuComponent,
    NoAccessComponent,
    AdminPanelComponent,
    NavAdministratorMenuComponent,
    BankAccountSummaryComponent,
    CustomerProfileComponent,
    BankTransferComponent,
    NewBranchComponent,
    AddressFormComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    MaterialModule,
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
        path: 'registration/customer', component: CustomerRegistrationComponent
      },
      {
        path: 'auth/login', component: LoginComponent
      },
      {
        path: 'no-access', component: NoAccessComponent
      },
      {
        path: 'admin-panel', component: AdminPanelComponent, canActivate: [AdminAuthGuard, AuthGuard]
      },
      {
        path: 'customer/profile', component: CustomerProfileComponent
      },
      {
        path: 'customer/bank-transfer', component: BankTransferComponent
      },
      {
        path: 'branch/new', component: NewBranchComponent, canActivate: [AdminAuthGuard, AuthGuard]
      }
    ])
  ],
  providers: [
    AuthService,
    AuthGuard,
    AdminAuthGuard
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
