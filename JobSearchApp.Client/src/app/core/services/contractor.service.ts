import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Contractor } from '../models/contractor.model';
import { PagedResult } from '../models';

@Injectable({ providedIn: 'root' })
export class ContractorService extends ApiService {
  private path = `${this.baseUrl}/contractor`;

  search(filter: string, pageNumber = 1, pageSize = 10): Observable<PagedResult<Contractor>> {
    const params = this.buildParams({ filter, pageNumber, pageSize });
    return this.http.get<PagedResult<Contractor>>(`${this.path}/search`, { params });
  }

  getById(id: string): Observable<Contractor> {
    return this.http.get<Contractor>(`${this.path}/${id}`);
  }

  create(contractor: Omit<Contractor, 'id'>): Observable<Contractor> {
    return this.http.post<Contractor>(this.path, contractor);
  }

  update(id: string, contractor: Partial<Contractor>): Observable<Contractor> {
    return this.http.put<Contractor>(`${this.path}/${id}`, contractor);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.path}/${id}`);
  }
}
