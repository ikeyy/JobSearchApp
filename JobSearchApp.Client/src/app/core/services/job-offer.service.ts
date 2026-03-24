import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from './api.service';
import { JobOffer } from '../models/job-offer.model';

export interface JobOfferPage {
  items: JobOffer[];
  total: number;
  pageNumber: number;
  pageSize: number;
}

export interface UpdateJobOfferStatusParams {
  jobOfferId?: string;
  customerId?: string;
  status?: string;
}

@Injectable({ providedIn: 'root' })
export class JobOfferService extends ApiService {
  private path = `${this.baseUrl}/joboffer`;

  getByJob(jobId: string, pageNumber = 1, pageSize = 10): Observable<JobOfferPage> {
    return this.http.get<JobOfferPage>(`${this.path}/search?filter=${jobId}&pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  getByContractor(contractorId: string, pageNumber = 1, pageSize = 10): Observable<JobOfferPage> {
    return this.http.get<JobOfferPage>(`${this.path}/search?filter=${contractorId}&pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  getById(id: string): Observable<JobOffer> {
    return this.http.get<JobOfferPage>(`${this.path}/search?filter=${id}`).pipe(
      map(res => res.items?.[0])
    );
  }

  create(offer: { jobId: string; contractorId: string; price: number }): Observable<JobOffer> {
    return this.http.post<JobOffer>(`${this.path}/create`, offer);
  }

  accept(params: UpdateJobOfferStatusParams = {}): Observable<JobOffer> {
    return this.http.put<JobOffer>(`${this.path}/status`, params );
  }

  reject(params: UpdateJobOfferStatusParams = {}): Observable<JobOffer> {
    return this.http.put<JobOffer>(`${this.path}/status`, params );
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.path}/delete/${id}`);
  }
}
