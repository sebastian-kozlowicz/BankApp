import { Page } from '@playwright/test';

export class AppPage {
  constructor(private readonly page: Page) {}

  navigateTo() {
    return this.page.goto('/');
  }

  getMainHeading() {
    return this.page.locator('app-home');
  }
}
