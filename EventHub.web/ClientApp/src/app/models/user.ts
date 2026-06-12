export interface User {
  id: number;
  fullName: string;
  email: string;
  role: 'Admin' | 'Organizer' | 'Attendee';
  status: string;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  fullName: string;
  email: string;
  password: string;
}

export interface TokenResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
}

export interface UpdateProfileDto {
  fullName: string;
}
