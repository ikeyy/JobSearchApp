import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-stars',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './stars.component.html',
  styleUrl: './stars.component.css',
})
export class StarsComponent {
  @Input() rating = 0;
  stars = [1, 2, 3, 4, 5];
}
