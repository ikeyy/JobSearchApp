import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent),
  },
  {
    path: 'customers',
    loadComponent: () => import('./features/customers/customers.component').then(m => m.CustomersComponent),
  },
  {
    path: 'contractors',
    loadComponent: () => import('./features/contractors/contractors.component').then(m => m.ContractorsComponent),
  },
  {
    path: 'jobs',
    loadComponent: () => import('./features/jobs/jobs.component').then(m => m.JobsComponent),
  },
  {
    path: 'job-offers',
    loadComponent: () => import('./features/job-offers/job-offers.component').then(m => m.JobOffersComponent),
  },
  { path: '**', redirectTo: 'dashboard' },
];
