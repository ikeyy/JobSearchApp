import { Component, EventEmitter, Input, OnChanges, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.css',
})
export class PaginationComponent implements OnChanges {
  @Input() page = 1;
  @Input() pageSize = 20;
  @Input() total = 0;
  @Output() pageChange = new EventEmitter<number>();

  totalPages = 0;
  pages: number[] = [];
  Math = Math;

  ngOnChanges() {
    this.totalPages = Math.ceil(this.total / this.pageSize);
    this.buildPages();
  }

  private buildPages() {
    const p = this.page, last = this.totalPages;
    const all: number[] = [];
    for (let i = 1; i <= last; i++) {
      if (i === 1 || i === last || Math.abs(i - p) <= 1) all.push(i);
      else if (all[all.length - 1] !== -1) all.push(-1);
    }
    this.pages = all;
  }

  go(p: number) {
    if (p >= 1 && p <= this.totalPages) this.pageChange.emit(p);
  }
}
