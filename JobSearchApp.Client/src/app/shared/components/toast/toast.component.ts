import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService, Toast } from '../../../core/services/toast.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './toast.component.html',
  styleUrl: './toast.component.css'
})
export class ToastComponent {
  toasts$: Observable<Toast[]>;
  constructor(private toastService: ToastService) {
    this.toasts$ = this.toastService.toasts$;
  }
  trackById(_: number, t: Toast) { return t.id; }
  dismiss(id: string) { this.toastService.remove(id); }
}
