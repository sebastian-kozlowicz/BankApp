import { NgModule } from '@angular/core';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { AppModule } from '../app.module';

// Exercise real templates and application providers without making network calls.
@NgModule({
  imports: [AppModule, NoopAnimationsModule],
  exports: [AppModule],
  providers: [provideHttpClientTesting()]
})
export class AppTestingModule {}
