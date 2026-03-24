import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged, switchMap, takeUntil } from 'rxjs';
import { CustomerService } from '../../core/services/customer.service';
import { ToastService } from '../../core/services/toast.service';
import { PagedResult } from '../../core/models';
import { Customer } from '../../core/models/customer.model';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { ConfirmDialogComponent } from '../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-customers',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, PaginationComponent, ConfirmDialogComponent],
  templateUrl: './customers.component.html',
  styleUrl: './customers.component.css',
})
export class CustomersComponent implements OnInit, OnDestroy {
  result: PagedResult<Customer> = { items: [], total: 0, pageNumber: 1, pageSize: 20 };
  loading = false;
  saving = false;
  pageNumber = 1;
  pageSize = 10;
  searchQuery = '';
  showModal = false;
  editingId: string | null = null;
  deleteTarget: Customer | null = null;
  form: FormGroup;
  private search$ = new Subject<string>();
  private destroy$ = new Subject<void>();

  constructor(private svc: CustomerService, private toast: ToastService, private fb: FormBuilder) {
    this.form = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
    });
  }

  ngOnInit() {
    this.search$.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(q => { this.loading = true; return this.svc.search(q, this.pageNumber, this.pageSize); }),
      takeUntil(this.destroy$)
    ).subscribe({ next: r => { this.result = r; this.loading = false; }, error: () => this.loading = false });
    this.load();
  }

  ngOnDestroy() { this.destroy$.next(); this.destroy$.complete(); }

  load() { this.search$.next(this.searchQuery); }
  onSearch(q: string) { this.pageNumber = 1; this.search$.next(q); }
  onPageChange(p: number) { this.pageNumber = p; this.load(); }

  openCreate() { this.editingId = null; this.form.reset(); this.showModal = true; }
  openEdit(c: Customer) { this.editingId = c.customerId; this.form.patchValue(c); this.showModal = true; }
  closeModal() { this.showModal = false; }

  submit() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.saving = true;
    const val = this.form.value;
    const req = this.editingId
      ? this.svc.update(this.editingId, val)
      : this.svc.create(val);
    req.subscribe({
      next: () => {
        this.toast.success(this.editingId ? 'Customer updated' : 'Customer created');
        this.closeModal();
        this.load();
        this.saving = false;
      },
      error: () => { this.toast.error('Operation failed'); this.saving = false; }
    });
  }

  confirmDelete(c: Customer) { this.deleteTarget = c; }
  doDelete() {
    if (!this.deleteTarget) return;
    this.svc.delete(this.deleteTarget.customerId).subscribe({
      next: () => { this.toast.success('Customer deleted'); this.deleteTarget = null; this.load(); },
      error: () => { this.toast.error('Delete failed'); this.deleteTarget = null; }
    });
  }
}
