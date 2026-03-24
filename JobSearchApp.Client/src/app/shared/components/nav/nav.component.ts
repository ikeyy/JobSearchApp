import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-nav',
  standalone: true,
  imports: [CommonModule, RouterModule, RouterLinkActive],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css',
})
export class NavComponent {}
