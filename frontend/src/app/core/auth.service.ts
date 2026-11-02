import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { Observable, map, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { LoginResponse, UserModel } from '../models/auth.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly tokenKey = 'standard-interiors.auth.token';
  private readonly userKey = 'standard-interiors.auth.user';
  readonly user = signal<UserModel | null>(this.readUser());

  login(username: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${environment.apiBaseUrl}/auth/login`, { username, password }).pipe(
      map((response) => ({ ...response, user: this.normalizeUser(response.user) })),
      tap((response) => {
        localStorage.setItem(this.tokenKey, response.token);
        localStorage.setItem(this.userKey, JSON.stringify(response.user));
        this.user.set(response.user);
      })
    );
  }

  refreshCurrentUser(): Observable<UserModel> {
    return this.http.get<UserModel>(`${environment.apiBaseUrl}/auth/me`).pipe(map((user) => this.normalizeUser(user)), tap((user) => this.setUser(user)));
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    this.user.set(null);
  }

  token(): string | null { return localStorage.getItem(this.tokenKey); }
  isAuthenticated(): boolean { return !!this.token(); }
  hasRole(...roles: string[]): boolean { return !!this.user() && roles.includes(this.user()!.role); }
  canWrite(): boolean { return this.hasRole('Admin', 'Staff'); }
  isAdmin(): boolean { return this.hasRole('Admin'); }
  canVoidPayments(): boolean { return this.isAdmin() || (!!this.user()?.canVoidPayments && this.hasRole('Staff')); }

  private setUser(user: UserModel): void {
    localStorage.setItem(this.userKey, JSON.stringify(user));
    this.user.set(user);
  }

  private readUser(): UserModel | null {
    try {
      const value = localStorage.getItem(this.userKey);
      return value ? this.normalizeUser(JSON.parse(value) as UserModel) : null;
    } catch {
      return null;
    }
  }

  private normalizeUser(user: UserModel): UserModel {
    const role = typeof user.role === 'number' ? ({ 1: 'Admin', 2: 'Staff', 3: 'Viewer' } as const)[user.role] : user.role;
    return { ...user, role: role || 'Viewer' };
  }
}
