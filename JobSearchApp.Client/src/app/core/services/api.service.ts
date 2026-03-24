import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';

@Injectable({ providedIn: 'root' })
export class ApiService {
  protected readonly baseUrl = environment.apiUrl;

  constructor(protected http: HttpClient) {}

  protected buildParams(query: Record<string, any>): HttpParams {
    let params = new HttpParams();
    Object.entries(query).forEach(([k, v]) => {
      if (v !== null && v !== undefined && v !== '') {
        params = params.set(k, String(v));
      }
    });
    return params;
  }
}
