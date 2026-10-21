/**
 * Aplica só a resposta da chamada mais recente: uma resposta antiga que chega depois (troca rápida
 * de nível/classe) é descartada (F8 AC11). Falha deixa o último valor aplicado na tela.
 */
export function latestOnly<T>(apply: (value: T) => void): (request: Promise<T>) => void {
  let latest = 0;
  return (request) => {
    const call = ++latest;
    request.then(
      (value) => {
        if (call === latest) {
          apply(value);
        }
      },
      () => undefined,
    );
  };
}
