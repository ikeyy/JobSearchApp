export type JobStatus = 'Open' | 'Offered' | 'Accepted' | 'Completed' | 'Cancelled';

export interface Job {
  jobId: string;
  startDate: string;
  dueDate: string;
  budget: number;
  description: string;
  status: JobStatus;
  customerId: string;
  acceptedBy?: string;
  createdAt?: string;
}