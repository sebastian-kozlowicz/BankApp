import { defineConfig } from '@playwright/test';

export default defineConfig({
  testDir: './e2e/src',
  testMatch: '**/*.e2e-spec.ts',
  workers: 1,
  reporter: 'list',
  outputDir: '.angular/playwright-results',
  use: { baseURL: 'http://localhost:4200', channel: 'chrome', headless: true },
  webServer: {
    command: 'npm start',
    url: 'http://localhost:4200',
    reuseExistingServer: !process.env.CI,
    timeout: 120000
  }
});
