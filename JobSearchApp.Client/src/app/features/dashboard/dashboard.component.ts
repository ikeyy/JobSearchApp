import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { forkJoin } from 'rxjs';
import { JobService } from '../../core/services/job.service';
import { CustomerService } from '../../core/services/customer.service';
import { ContractorService } from '../../core/services/contractor.service';
import { Job } from '../../core/models/job.model';

interface Stats {
  customers: number;
  contractors: number;
  jobs: number;
  openJobs: number;
  acceptedJobs: number;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
})
export class DashboardComponent implements OnInit {
  stats: Stats = { customers: 0, contractors: 0, jobs: 0, openJobs: 0, acceptedJobs: 0 };
  recentJobs: Job[] = [];
  loadingJobs = true;
  now = new Date();
  apiUrl = (window as any).__env?.apiUrl;

  constructor(
    private jobSvc: JobService,
    private customerSvc: CustomerService,
    private contractorSvc: ContractorService,
  ) {}

  ngOnInit() {
    forkJoin({
      jobs: this.jobSvc.search({ pageSize: 5 }),
      openJobs: this.jobSvc.search({ status: 'Open', pageSize: 1 }),
      acceptedJobs: this.jobSvc.search({ status: 'Accepted', pageSize: 1 }),
      customers: this.customerSvc.search('', 1, 1),
      contractors: this.contractorSvc.search('', 1, 1),
    }).subscribe({
      next: r => {
        this.stats = {
          customers: r.customers.total,
          contractors: r.contractors.total,
          jobs: r.jobs.total,
          openJobs: r.openJobs.total,
          acceptedJobs: r.acceptedJobs.total,
        };
        this.recentJobs = r.jobs.items;
        this.loadingJobs = false;
      },
      error: () => this.loadingJobs = false
    });
  }
}
