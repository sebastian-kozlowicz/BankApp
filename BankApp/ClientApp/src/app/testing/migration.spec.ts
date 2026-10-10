import { TestBed } from '@angular/core/testing';
import { ApplicationRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HttpTestingController } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AppTestingModule } from './app-testing.module';
import { AuthGuard } from '../services/auth-guard.service';

describe('Angular migration integration', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [AppTestingModule] });
    sessionStorage.removeItem('token');
  });
  afterEach(() => {
    TestBed.inject(HttpTestingController).verify();
    TestBed.inject(ToastrService).clear();
    sessionStorage.removeItem('token');
  });

  it('preserves all application route paths', () => {
    expect(TestBed.inject(Router).config.map(route => route.path)).toEqual([
      '', 'registration/customer', 'auth/login', 'no-access', 'user/new',
      'admin-panel', 'customer/profile', 'customer/bank-transfer', 'branch/new'
    ]);
  });

  it('protects customer profile and bank transfer routes with AuthGuard', () => {
    const customerRoutes = TestBed.inject(Router).config.filter(route =>
      route.path === 'customer/profile' || route.path === 'customer/bank-transfer');

    expect(customerRoutes).toHaveSize(2);
    customerRoutes.forEach(route => {
      expect(route.canActivate).toContain(AuthGuard);
    });
  });

  it('attaches the current JWT and picks up a replaced session token', () => {
    const http = TestBed.inject(HttpClient);
    const controller = TestBed.inject(HttpTestingController);
    for (const token of ['initial-session-token', 'refreshed-session-token']) {
      sessionStorage.setItem('token', token);
      http.get('/api/bankaccounts').subscribe();
      const request = controller.expectOne('/api/bankaccounts');
      expect(request.request.headers.get('Authorization')).toBe('Bearer ' + token);
      request.flush([]);
    }
  });

  it('does not leak JWTs to external domains', () => {
    sessionStorage.setItem('token', 'session-token');
    TestBed.inject(HttpClient).get('https://example.com/api').subscribe();
    const request = TestBed.inject(HttpTestingController).expectOne('https://example.com/api');
    expect(request.request.headers.has('Authorization')).toBeFalse();
    request.flush({});
  });

  it('displays and clears a toast using the upgraded library', () => {
    const toastr = TestBed.inject(ToastrService);
    const toast = toastr.success('Migration notification');
    TestBed.inject(ApplicationRef).tick();
    expect(document.body.textContent).toContain('Migration notification');
    toastr.clear(toast.toastId);
  });
});
