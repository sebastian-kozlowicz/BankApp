import { test, expect } from '@playwright/test';

for (const width of [1440, 768, 375]) {
  test(`home page renders without overflow at ${width}px`, async ({ page }) => {
    const errors: string[] = [];
    page.on('pageerror', error => errors.push(error.message));
    await page.setViewportSize({ width, height: 1000 });
    await page.goto('/');
    await expect(page.getByRole('heading', { level: 1 })).toContainText('Your money.');
    await expect(page.getByRole('link', { name: 'Get started' })).toBeVisible();
    await expect(page.locator('.feature-card')).toHaveCount(3);
    expect(await page.evaluate(() => document.documentElement.scrollWidth <= window.innerWidth)).toBe(true);
    await page.screenshot({ path: `.angular/playwright-results/home-${width}.png`, fullPage: true });
    await page.getByRole('link', { name: 'Sign in to BankApp' }).click();
    await expect(page).toHaveURL(/\/auth\/login$/);
    expect(errors).toEqual([]);
  });
}
