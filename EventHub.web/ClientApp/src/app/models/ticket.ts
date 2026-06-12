export interface Ticket {
  id: number;
  eventId: number;
  registrationId: number;
  userId: number;
  price: number;
  seatNumber: string;
  pdfPath: string;
  createdAt: string;
}
