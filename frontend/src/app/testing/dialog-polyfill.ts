/**
 * jsdom não implementa `<dialog>.showModal()`/`close()`. Basta abrir/fechar e emitir `close`;
 * foco, Esc e inércia do resto da página são conferidos no E2E (Chromium real).
 */
export function polyfillDialog(): void {
  const proto = HTMLDialogElement.prototype;
  proto.showModal ??= function (this: HTMLDialogElement) {
    this.open = true;
  };
  proto.close ??= function (this: HTMLDialogElement) {
    this.open = false;
    this.dispatchEvent(new Event('close'));
  };
}
