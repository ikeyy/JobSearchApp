import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { ToastService } from '../services/toast.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toast = inject(ToastService);

  return next(req).pipe(
    catchError(err => {
      const status = err.status;
      let message = 'An unexpected error occurred';

      if (status === 0) {
        message = 'Cannot reach the API server — check your connection';
      } else if (status === 400) {
        const body = err.error;
        if (body?.errors) {
          const fieldErrors = Object.values(body.errors as Record<string, string[]>)
            .flat()
            .slice(0, 2)
            .join('; ');
          message = fieldErrors || 'Validation failed';
        } else {
          message = body?.message || 'Bad request';
        }
      } else if (status === 404) {
        message = 'Resource not found';
      } else if (status === 409) {
        message = err.error?.message || 'Conflict — resource already exists';
      } else if (status === 422) {
        message = err.error?.message || 'Unprocessable entity';
      } else if (status >= 500) {
        message = 'Server error — please try again later';
      }

      // Only show toast for non-404 errors in background calls
      // (components show their own contextual errors for 404 lookups)
      if (status !== 404) {
        toast.error(message);
      }

      return throwError(() => ({ status, message, original: err }));
    })
  );
};
