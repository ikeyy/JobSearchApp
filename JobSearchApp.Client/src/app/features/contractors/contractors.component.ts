import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged, switchMap, takeUntil } from 'rxjs';
import { ContractorService } from '../../core/services/contractor.service';
import { ToastService } from '../../core/services/toast.service';
import { PagedResult } from '../../core/models';
import { Contractor} from '../../core/models/contractor.model';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { ConfirmDialogComponent } from '../../shared/components/confirm-dialog/confirm-dialog.component';
import { StarsComponent } from '../../shared/components/stars/stars.component';

@Component({
  selector: 'app-contractors',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, PaginationComponent, ConfirmDialogComponent, StarsComponent],
  templateUrl: './contractors.component.html',
  styleUrl: './contractors.component.css',
})
export class ContractorsComponent implements OnInit, OnDestroy {
  result: PagedResult<Contractor> = { items: [], total: 0, pageNumber: 1, pageSize: 20 };
  loading = false;
  saving = false;
  pageNumber = 1;
  pageSize = 10;
  searchQuery = '';
  showModal = false;
  editingId: string | null = null;
  deleteTarget: Contractor | null = null;
  form: FormGroup;
  private search$ = new Subject<string>();
  private destroy$ = new Subject<void>();

  constructor(private svc: ContractorService, private toast: ToastService, private fb: FormBuilder) {
    this.form = this.fb.group({
      name: ['', Validators.required],
      rating: [0, [Validators.required, Validators.min(0), Validators.max(5)]],
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
  openCreate() { this.editingId = null; this.form.reset({ rating: 0 }); this.showModal = true; }
  openEdit(c: Contractor) { this.editingId = c.contractorId; this.form.patchValue(c); this.showModal = true; }
  closeModal() { this.showModal = false; }

  submit() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.saving = true;
    const val = this.form.value;
    const req = this.editingId
      ? this.svc.update(this.editingId, val)
      : this.svc.create(val);
    req.subscribe({
      next: () => { this.toast.success(this.editingId ? 'Contractor updated' : 'Contractor created'); this.closeModal(); this.load(); this.saving = false; },
      error: () => { this.toast.error('Operation failed'); this.saving = false; }
    });
  }

  confirmDelete(c: Contractor) { this.deleteTarget = c; }
  doDelete() {
    if (!this.deleteTarget) return;
    this.svc.delete(this.deleteTarget.contractorId).subscribe({
      next: () => { this.toast.success('Contractor deleted'); this.deleteTarget = null; this.load(); },
      error: () => { this.toast.error('Delete failed'); this.deleteTarget = null; }
    });
  }
}
