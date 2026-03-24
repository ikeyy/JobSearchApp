import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject } from 'rxjs';
import { JobOfferService } from '../../core/services/job-offer.service';
import { ToastService } from '../../core/services/toast.service';
import { JobOffer } from '../../core/models/job-offer.model';
import { ConfirmDialogComponent } from '../../shared/components/confirm-dialog/confirm-dialog.component';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-job-offers',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, ConfirmDialogComponent, PaginationComponent],
  templateUrl: './job-offers.component.html',
  styleUrl: './job-offers.component.css',
})
export class JobOffersComponent implements OnInit, OnDestroy {
  offers: JobOffer[] = [];
  loading = false; saving = false;
  showModal = false;
  deleteTarget: JobOffer | null = null;
  lookupJobId = ''; lookupContractorId = ''; lookupLabel = '';

  // Pagination state
  page = 1;
  pageSize = 10;
  total = 0;
  private activeFilter: 'job' | 'contractor' | null = null;

  form: FormGroup;
  private destroy$ = new Subject<void>();

  constructor(private svc: JobOfferService, private toast: ToastService, private fb: FormBuilder) {
    this.form = this.fb.group({
      jobId: ['', Validators.required],
      contractorId: ['', Validators.required],
      price: [null, [Validators.required, Validators.min(1)]],
    });
  }

  ngOnInit() {}
  ngOnDestroy() { this.destroy$.next(); this.destroy$.complete(); }

  statusClass(s: string) { return `badge-${s.toLowerCase()}`; }

  lookupByJob() {
    if (!this.lookupJobId) return;
    this.activeFilter = 'job';
    this.page = 1;
    this.lookupLabel = `Offers for job ${this.lookupJobId}`;
    this.fetchPage();
  }

  lookupByContractor() {
    if (!this.lookupContractorId) return;
    this.activeFilter = 'contractor';
    this.page = 1;
    this.lookupLabel = `Offers by contractor ${this.lookupContractorId}`;
    this.fetchPage();
  }

  onPageChange(p: number) {
    this.page = p;
    this.fetchPage();
  }

  private fetchPage() {
    this.loading = true;
    const obs = this.activeFilter === 'job'
      ? this.svc.getByJob(this.lookupJobId, this.page, this.pageSize)
      : this.svc.getByContractor(this.lookupContractorId, this.page, this.pageSize);

    obs.subscribe({
      next: res => {
        // Guard: ensure items is always an array regardless of API shape
        this.offers = Array.isArray(res.items) ? res.items : [];
        this.total = res.total ?? 0;
        this.loading = false;
      },
      error: () => { this.toast.error('Lookup failed'); this.loading = false; }
    });
  }

  openCreate() { this.form.reset(); this.showModal = true; }
  closeModal() { this.showModal = false; }

  submit() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.saving = true;
    this.svc.create(this.form.value).subscribe({
      next: o => {
        this.toast.success('Offer submitted');
        this.offers = [o, ...this.offers];
        this.closeModal();
        this.saving = false;
      },
      error: () => { this.toast.error('Submission failed'); this.saving = false; }
    });
  }

  accept(o: JobOffer) {
    this.svc.accept({ jobOfferId: o.jobOfferId, customerId: '3E859D28-8B86-44C8-9105-1A34B265621E', status: 'Accepted' }).subscribe({
      next: updated => {
        this.toast.success('Offer accepted');
        this.offers = this.offers.map(x => x.jobOfferId === o.jobOfferId ? updated : x);
      },
      error: () => this.toast.error('Failed to accept')
    });
  }

  reject(o: JobOffer) {
    this.svc.reject({ jobOfferId: o.jobOfferId, customerId: '3E859D28-8B86-44C8-9105-1A34B265621E', status: 'Rejected' }).subscribe({
      next: updated => {
        this.toast.success('Offer rejected');
        this.offers = this.offers.map(x => x.jobOfferId === o.jobOfferId ? updated : x);
      },
      error: () => this.toast.error('Failed to reject')
    });
  }

  confirmDelete(o: JobOffer) { this.deleteTarget = o; }
  doDelete() {
    if (!this.deleteTarget) return;
    this.svc.delete(this.deleteTarget.jobOfferId).subscribe({
      next: () => {
        this.toast.success('Offer deleted');
        this.offers = this.offers.filter(o => o.jobOfferId !== this.deleteTarget!.jobOfferId);
        this.deleteTarget = null;
      },
      error: () => { this.toast.error('Delete failed'); this.deleteTarget = null; }
    });
  }
}
