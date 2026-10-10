import { test, expect } from '@playwright/test';
import { AppPage } from './app.po';

test('should display the home welcome content without runtime errors', async ({ page }) => {
  const errors: string[] = [];
  page.on('pageerror', error => errors.push(error.message));
  const app = new AppPage(page);
  await app.navigateTo();
  await expect(app.getMainHeading()).toContainText('Home');
  await expect(page.getByRole('link', { name: 'Login', exact: true })).toBeVisible();
  expect(errors).toEqual([]);
});

test('preserves login navigation and form validation', async ({ page }) => {
  await page.goto('/auth/login');
  await expect(page.getByRole('button', { name: 'Sign In' })).toBeDisabled();
  await page.getByLabel('Email', { exact: true }).fill('customer@example.com');
  await page.getByLabel('Password', { exact: true }).fill('Sample-Test-Input');
  await expect(page.getByRole('button', { name: 'Sign In' })).toBeEnabled();
});

test('renders the registration Material stepper and custom forms', async ({ page }) => {
  await page.goto('/registration/customer');
  await expect(page.getByRole('button', { name: 'Account information', exact: true })).toBeVisible();
  await expect(page.getByRole('combobox').first()).toBeVisible();
});
