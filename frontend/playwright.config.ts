import { defineConfig, devices } from '@playwright/test';
import { resolve } from 'node:path';

/**
 * E2E do wizard contra a API real (ADR-0009). API e front sobem isolados dos servidores de dev:
 * API na 5180 com fichas em `e2e/.data/`, front na 4300.
 */
export const E2E_API = 'http://localhost:5180';
const FRONT = 'http://localhost:4300';
const sheetsDir = resolve(__dirname, 'e2e/.data');

export default defineConfig({
  testDir: 'e2e',
  globalSetup: './e2e/global-setup.ts',
  fullyParallel: false,
  workers: 1,
  forbidOnly: !!process.env['CI'],
  retries: process.env['CI'] ? 1 : 0,
  reporter: process.env['CI'] ? 'github' : 'list',
  use: {
    baseURL: FRONT,
    locale: 'pt-BR',
    timezoneId: 'America/Sao_Paulo',
    // Sem transições: o axe mede o contraste do estado final, não de um quadro intermediário.
    reducedMotion: 'reduce',
    trace: 'retain-on-failure',
  },
  projects: [{ name: 'chromium', use: { ...devices['Desktop Chrome'] } }],
  webServer: [
    {
      command: `dotnet run --project ../backend/src/DndBuilder.Api --no-launch-profile --urls ${E2E_API}`,
      url: `${E2E_API}/health`,
      env: {
        ASPNETCORE_ENVIRONMENT: 'Development',
        Storage__SheetsPath: sheetsDir,
        Frontend__Origin: FRONT,
      },
      reuseExistingServer: false,
      timeout: 180_000,
    },
    {
      command: 'npx ng serve --port 4300',
      url: FRONT,
      reuseExistingServer: false,
      timeout: 180_000,
    },
  ],
});
