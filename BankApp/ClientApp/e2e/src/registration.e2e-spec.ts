import { test, expect } from '@playwright/test';

for (const width of [1440, 768, 375]) {
  test(`customer registration works through all steps at ${width}px`, async ({ page }) => {
    const errors: string[] = [];
    page.on('pageerror', error => errors.push(error.message));
    // Never create a real customer during UI verification.
    await page.route('**/api/BankAccounts/CreateWithCustomerByCustomer', route => route.abort('failed'));
    await page.setViewportSize({ width, height: 1000 });
    await page.emulateMedia({ reducedMotion: 'reduce' });
    await page.goto('/registration/customer');
    await expect(page.getByRole('heading', { level: 1 })).toHaveText('Create your BankApp account');
    await expect(page.getByRole('button', { name: 'Continue', exact: true })).toBeDisabled();
    await page.screenshot({ path: `.angular/registration-screenshots/account-${width}.png`, fullPage: true });

    await page.getByRole('radio', { name: /Checking account/ }).check();
    await page.locator('#registration-currency').selectOption({ label: 'Polish złoty' });
    await page.getByRole('button', { name: 'Continue', exact: true }).click();
    await expect(page.locator('#name')).toBeVisible();
    await page.locator('#name').fill('Test');
    await page.locator('#surname').fill('Customer');
    await page.locator('#phone-number').fill('001234567');
    await page.locator('#email').fill('customer@example.test');
    await page.screenshot({ path: `.angular/registration-screenshots/personal-${width}.png`, fullPage: true });
    await page.locator('app-registration form').nth(1).getByRole('button', { name: 'Continue', exact: true }).click();

    await page.locator('#street').fill('Test street');
    await page.locator('#house-number').fill('1');
    await page.locator('#postal-code').fill('01-001');
    await page.locator('#city').fill('Poznan');
    await page.locator('#country').click();
    await page.getByRole('option', { name: 'Poland', exact: true }).click();
    await page.screenshot({ path: `.angular/registration-screenshots/address-${width}.png`, fullPage: true });
    // Apartment number is optional, in agreement with the backend DTO.
    await page.locator('app-registration form').nth(2).getByRole('button', { name: 'Continue', exact: true }).click();
    await expect(page.locator('#password')).toBeVisible();
    await page.locator('#password').fill('Test-only-123!');
    await page.locator('#confirm-password').fill('Does-not-match-123!');
    await page.locator('#confirm-password').blur();
    await expect(page.getByText('Passwords do not match.', { exact: true })).toBeVisible();
    await expect(page.getByRole('button', { name: 'Create account' })).toBeDisabled();
    await page.locator('#confirm-password').fill('Test-only-123!');
    await expect(page.getByRole('button', { name: 'Create account' })).toBeEnabled();
    await page.screenshot({ path: `.angular/registration-screenshots/password-${width}.png`, fullPage: true });

    expect(await page.evaluate(() => document.documentElement.scrollWidth <= window.innerWidth)).toBe(true);
    await page.locator('app-registration form').nth(3).getByRole('button', { name: 'Back', exact: true }).click();
    await expect(page.locator('#postal-code')).toHaveValue('01-001');
    await page.locator('app-registration form').nth(2).getByRole('button', { name: 'Continue', exact: true }).click();
    await page.getByRole('button', { name: 'Create account' }).click();
    await expect(page.locator('.request-error')).toContainText('Registration failed');
    await expect(page.getByRole('button', { name: 'Create account' })).toBeEnabled();
    expect(errors).toEqual([]);
  });
}
