export interface Payment {
  id: number;
  registrationId: number;
  eventTitle: string;
  paymentMethod: string;
  amount: number;
  status: string;
  paidAt: string;
}

export interface PaymentCreateDto {
  registrationId: number;
  amount: number;
  paymentMethod: string;
}
