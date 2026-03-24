export interface PagedResult<T> {
  items: T[];
  total: number;
  pageNumber: number;
  pageSize: number;
}

export interface ApiError {
  message: string;
  errors?: Record<string, string[]>;
}
