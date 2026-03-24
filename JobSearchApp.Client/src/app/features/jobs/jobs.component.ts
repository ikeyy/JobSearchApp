import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject, debounceTime, takeUntil, switchMap } from 'rxjs';
import { JobService } from '../../core/services/job.service';
import { JobOfferPage, JobOfferService } from '../../core/services/job-offer.service';
import { ToastService } from '../../core/services/toast.service';
import { PagedResult } from '../../core/models';
import { JobOffer } from '../../core/models/job-offer.model';
import { Job, JobStatus } from '../../core/models/job.model';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { ConfirmDialogComponent } from '../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-jobs',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, PaginationComponent, ConfirmDialogComponent],
  templateUrl: './jobs.component.html',
  styleUrl: './jobs.component.css',
})
export class JobsComponent implements OnInit, OnDestroy {
  result: PagedResult<Job> = { items: [], total: 0, pageNumber: 1, pageSize: 10 };
  loading = false; saving = false;
  pageNumber = 1; 
  pageSize = 10;
  searchQuery = ''; statusFilter = ''; 
  minBudget?: number; 
  maxBudget?: number;
  showModal = false; editingId: string | null = null;
  deleteTarget: Job | null = null;
  offersJob: Job | null = null;
  offers: JobOffer[] = [];
  offersLoading = false;
  form: FormGroup;
  private search$ = new Subject<void>();
  private destroy$ = new Subject<void>();

  constructor(
    private svc: JobService,
    private offerSvc: JobOfferService,
    private toast: ToastService,
    private fb: FormBuilder
  ) {
    this.form = this.fb.group({
      description: ['', [Validators.required, Validators.minLength(10)]],
      startDate: ['', Validators.required],
      dueDate: ['', Validators.required],
      budget: [null, [Validators.required, Validators.min(1)]],
      customerId: ['', Validators.required],
      status: ['Open'],
    });
  }

  ngOnInit() {
    this.search$.pipe(
      debounceTime(300),
      switchMap(() => {
        this.loading = true;
        return this.svc.search({
          description: this.searchQuery,
          status: this.statusFilter || undefined,
          minBudget: this.minBudget,
          maxBudget: this.maxBudget,
          pageNumber: this.pageNumber,
          pageSize: this.pageSize,
        });
      }),
      takeUntil(this.destroy$)
    ).subscribe({ next: r => { this.result = r; this.loading = false; }, error: () => this.loading = false });
    this.load();
  }

  ngOnDestroy() { this.destroy$.next(); this.destroy$.complete(); }
  load() { this.search$.next(); }
  onSearch(_q: string) { this.pageNumber = 1; this.search$.next(); }
  onFilter() { this.pageNumber = 1; this.search$.next(); }
  onPageChange(p: number) { this.pageNumber = p; this.load(); }

  statusClass(s: JobStatus): string { return `badge-${s.toLowerCase()}`; }
  offerStatusClass(s: string): string { return `badge-${s.toLowerCase()}`; }

  openCreate() { this.editingId = null; this.form.reset({ status: 'Open' }); this.showModal = true; }
  openEdit(j: Job) {
    this.editingId = j.jobId;
    this.form.patchValue({
      ...j,
      startDate: j.startDate?.slice(0, 10),
      dueDate: j.dueDate?.slice(0, 10),
    });
    this.showModal = true;
  }
  closeModal() { this.showModal = false; }

  submit() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.saving = true;
    const val = this.form.value;
    const req = this.editingId ? this.svc.update(this.editingId, val) : this.svc.create(val);
    req.subscribe({
      next: () => { this.toast.success(this.editingId ? 'Job updated' : 'Job posted'); this.closeModal(); this.load(); this.saving = false; },
      error: () => { this.toast.error('Operation failed'); this.saving = false; }
    });
  }

  viewOffers(j: Job) {
    this.offersJob = j;
    this.offersLoading = true;
    this.offerSvc.getByJob(j.jobId).subscribe({
      next: o => { this.offers = o.items; this.offersLoading = false; },
      error: () => this.offersLoading = false
    });
  }
  closeOffers() { this.offersJob = null; this.offers = []; }

  acceptOffer(o: JobOffer) {
    this.offerSvc.accept({ jobOfferId: o.jobOfferId, customerId: '3E859D28-8B86-44C8-9105-1A34B265621E', status: 'Accepted' }).subscribe({
      next: () => { this.toast.success('Offer accepted'); this.viewOffers(this.offersJob!); this.load(); },
      error: () => this.toast.error('Failed to accept')
    });
  }
  rejectOffer(o: JobOffer) {
    this.offerSvc.reject({ jobOfferId: o.jobOfferId, customerId: '3E859D28-8B86-44C8-9105-1A34B265621E', status: 'Rejected' }).subscribe({
      next: () => { this.toast.success('Offer rejected'); this.viewOffers(this.offersJob!); },
      error: () => this.toast.error('Failed to reject')
    });
  }

  confirmDelete(j: Job) { this.deleteTarget = j; }
  doDelete() {
    if (!this.deleteTarget) return;
    this.svc.delete(this.deleteTarget.jobId).subscribe({
      next: () => { this.toast.success('Job deleted'); this.deleteTarget = null; this.load(); },
      error: () => { this.toast.error('Delete failed'); this.deleteTarget = null; }
    });
  }
}
