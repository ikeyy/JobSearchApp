import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { PagedResult } from '../models';
import { Customer } from '../models/customer.model';

@Injectable({ providedIn: 'root' })
export class CustomerService extends ApiService {
  private path = `${this.baseUrl}/customer`;

  search(filter: string, pageNumber = 1, pageSize = 10): Observable<PagedResult<Customer>> {
    const params = this.buildParams({ filter, pageNumber, pageSize });
    return this.http.get<PagedResult<Customer>>(`${this.path}/search`, { params });
  }

  getById(id: string): Observable<Customer> {
    return this.http.get<Customer>(`${this.path}/${id}`);
  }

  create(customer: Omit<Customer, 'customerId'>): Observable<Customer> {
    return this.http.post<Customer>(this.path, customer);
  }

  update(id: string, customer: Partial<Customer>): Observable<Customer> {
    return this.http.put<Customer>(`${this.path}/${id}`, customer);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.path}/${id}`);
  }
}
