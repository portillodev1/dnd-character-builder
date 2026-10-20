import { mkdirSync, rmSync } from 'node:fs';
import { resolve } from 'node:path';

/** Cada execução começa com a pasta de fichas do E2E vazia. */
export default function globalSetup(): void {
  const dir = resolve(__dirname, '.data');
  rmSync(dir, { recursive: true, force: true });
  mkdirSync(dir, { recursive: true });
}
