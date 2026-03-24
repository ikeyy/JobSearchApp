import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { PagedResult } from '../models';
import { Job } from '../models/job.model';

export interface JobSearchParams {
  description?: string;
  status?: string;
  minBudget?: number;
  maxBudget?: number;
  pageNumber?: number;
  pageSize?: number;
}

@Injectable({ providedIn: 'root' })
export class JobService extends ApiService {
  private path = `${this.baseUrl}/job`;

  search(params: JobSearchParams = {}): Observable<PagedResult<Job>> {
    const httpParams = this.buildParams({ ...params, pageNumber: params.pageNumber ?? 1, pageSize: params.pageSize ?? 10 });
    return this.http.get<PagedResult<Job>>(`${this.path}/search`, { params: httpParams });
  }

  getById(id: string): Observable<Job> {
    return this.http.get<Job>(`${this.path}/${id}`);
  }

  create(job: Omit<Job, 'jobId' | 'createdAt'>): Observable<Job> {
    return this.http.post<Job>(`${this.path}/create`, job);
  }

  update(id: string, job: Partial<Job>): Observable<Job> {
    return this.http.put<Job>(`${this.path}/update/${id}`, job);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.path}/delete/${id}`);
  }

  getOffersForJob(jobId: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.path}/${jobId}/offers`);
  }
}
