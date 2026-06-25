export interface Registration {
  id: number;
  eventId: number;
  eventTitle: string;
  attendeeId: number;
  attendeeName: string;
  registrationDate: string;
  status: string;
  eventEndDate: string;
  eventStatus: string;
}

export interface RegistrationCreateDto {
  eventId: number;
}
