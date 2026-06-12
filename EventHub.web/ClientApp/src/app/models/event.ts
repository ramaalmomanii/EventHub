export interface Event {
  id: number;
  organizerId: number;
  organizerName: string;
  title: string;
  description: string;
  categoryId: number;
  categoryName: string;
  startDate: string;
  endDate: string;
  location: string;
  price: number;
  capacity: number;
  availableSeats: number;
  status: string;
  createdAt: string;
}

export interface EventCreateDto {
  title: string;
  description: string;
  categoryId: number;
  startDate: string;
  endDate: string;
  location: string;
  price: number;
  capacity: number;
}

export interface EventUpdateDto {
  title: string;
  description: string;
  startDate: string;
  endDate: string;
  location: string;
  price: number;
  capacity: number;
}
