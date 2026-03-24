import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavComponent } from './shared/components/nav/nav.component';
import { ToastComponent } from './shared/components/toast/toast.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavComponent, ToastComponent],
  templateUrl: './app.component.html',
})
export class AppComponent {}
