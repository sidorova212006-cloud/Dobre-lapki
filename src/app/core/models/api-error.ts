/** Единый формат ошибки, который возвращает API. */
export interface ErrorResponse {
  code: string;
  message: string;
  traceId: string;
}

/** Ошибка HTTP-запроса с кодом ответа и адресом. */
export class ApiError extends Error {
  constructor(
    message: string,
    readonly status: number,
    readonly url: string,
  ) {
    super(message);
    this.name = 'ApiError';
  }
}
