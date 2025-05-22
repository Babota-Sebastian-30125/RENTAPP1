import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { LoginResponse } from '../models/LoginResponseModel';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private userNameSubject = new BehaviorSubject<string | null>(localStorage.getItem('name'));
  userName$ = this.userNameSubject.asObservable();
  private apiUrl = `${environment.apiUrl}/auth`;

  constructor(private http: HttpClient) { }

  login(email: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, { email, password }).pipe(
      tap((res: LoginResponse) => {
        localStorage.setItem('token', res.token);
        localStorage.setItem('name', res.name);
        localStorage.setItem('email', res.email);
        localStorage.setItem('telephoneNumber', res.telephoneNumber);
        this.userNameSubject.next(res.name);
      }),
      catchError(this.handleError)
    );
  }

  register(name: string, email: string, telephoneNumber: string, password: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, { name, email, telephoneNumber, password }).pipe(
      catchError(this.handleError)
    );
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('name');
    localStorage.removeItem('email');
    localStorage.removeItem('telephoneNumber');
    this.userNameSubject.next(null);
  }

  getUserName(): string | null {
    return this.userNameSubject.value;
  }

  updateAccount(name: string, email: string, telephoneNumber: string): Observable<any> {
    const token = this.getToken();
    if (!token) {
      return throwError(() => new Error('Nu exista token valid'));
    }

    return this.http.put<any>(
      `${this.apiUrl}/update-account`,
      { name, email, telephoneNumber },
      {
        headers: new HttpHeaders({
          Authorization: `Bearer ${token}`
        })
      }
    ).pipe(
      tap(() => {
        localStorage.setItem('name', name);
        localStorage.setItem('email', email);
        localStorage.setItem('telephoneNumber', telephoneNumber);
      }),
      catchError(this.handleError)
    );
  }

  changePassword(currentPassword: string, newPassword: string): Observable<any> {
    const token = this.getToken();
    if (!token) {
      return throwError(() => new Error('Token invalid.'));
    }

    const body = { oldPassword: currentPassword, newPassword };

    return this.http.put(`${environment.apiUrl}/user/change-password`, body, {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
      })
    }).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse) {
    console.error('Eroare API:', error);
    // msj backend
    if (error.error instanceof Object && error.error.Errors) {
   
      const messages = error.error.Errors.map((e: any) => `${e.Field}: ${e.Errors.join(', ')}`).join(' | ');
      return throwError(() => new Error(messages));
    } else if (error.error && typeof error.error === 'string') {
     
      return throwError(() => new Error(error.error));
    } else {
      return throwError(() => new Error('A aparut o eroare'));
    }
  }
  deleteAccount(): Observable<any> {
    const token = this.getToken();
    if (!token) {
      return throwError(() => new Error('Token invalid.'));
    }

    return this.http.delete(`${environment.apiUrl}/User/delete-account`, {
      headers: new HttpHeaders({
        Authorization: `Bearer ${token}`
      })
    }).pipe(
      tap(() => {
        this.logout();
      }),
      catchError(this.handleError)
    );
  }

}
