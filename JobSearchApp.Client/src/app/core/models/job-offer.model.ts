export interface JobOffer {
  jobOfferId: string;
  jobId: string;
  contractorId: string;
  contractorName?: string;
  price: number;
  status: 'Pending' | 'Accepted' | 'Rejected';
  createdAt?: string;
}