import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface Toast {
  id: string;
  type: 'success' | 'error' | 'info' | 'warning';
  message: string;
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private toastsSubject = new BehaviorSubject<Toast[]>([]);
  toasts$ = this.toastsSubject.asObservable();

  private add(type: Toast['type'], message: string) {
    const id = Math.random().toString(36).slice(2);
    const toast: Toast = { id, type, message };
    this.toastsSubject.next([...this.toastsSubject.value, toast]);
    setTimeout(() => this.remove(id), 4000);
  }

  success(message: string) { this.add('success', message); }
  error(message: string) { this.add('error', message); }
  info(message: string) { this.add('info', message); }
  warning(message: string) { this.add('warning', message); }

  remove(id: string) {
    this.toastsSubject.next(this.toastsSubject.value.filter(t => t.id !== id));
  }
}
