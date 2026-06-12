export interface Registration {
  id: number;
  eventId: number;
  eventTitle: string;
  attendeeId: number;
  attendeeName: string;
  registrationDate: string;
  status: string;
}

export interface RegistrationCreateDto {
  eventId: number;
}
