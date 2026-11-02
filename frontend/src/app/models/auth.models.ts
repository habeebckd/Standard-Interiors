export type UserRole = 'Admin' | 'Staff' | 'Viewer';

export interface UserModel {
  id: number;
  username: string;
  displayName: string;
  role: UserRole;
  canVoidPayments: boolean;
  isActive: boolean;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
  user: UserModel;
}
