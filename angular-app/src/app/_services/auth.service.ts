import { Injectable } from '@angular/core';
import { AuthRequestResponse } from '../_models/auth-request-response';
import { GenericService } from './generic-service';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, tap } from 'rxjs';
import { CommonUtilities } from '../_common/common-utilities';

@Injectable({
  providedIn: 'root'
})
export class AuthService extends GenericService<AuthRequestResponse> {
  constructor(
    http: HttpClient
  ) {
    super(http);
  }

  get endpoint(): string {
    return '/api/auth';
  }

  public logout() {
    localStorage.removeItem('token');
    return this.http.post(`${this.endpoint}/logout`, null).pipe(
      catchError(err => CommonUtilities.handleHttpError(err))
    );
  }

  public login(username: string, password: string) {
    let data = new AuthRequestResponse();
    data.email = username;
    data.password = password;
    return this.http.post<AuthRequestResponse>(`${this.endpoint}/login`, data).pipe(
      tap(response => {
        localStorage.setItem('token', response.token); // Store the token
      }),
      catchError(err => CommonUtilities.handleHttpError<AuthRequestResponse>(err))
    );
  } 

  public register(username: string, password: string) {
    let data = new AuthRequestResponse();
    data.email = username;
    data.password = password;
    return this.http.post<AuthRequestResponse>(`${this.endpoint}/register`, data).pipe(
      catchError(err => CommonUtilities.handleHttpError<AuthRequestResponse>(err))
    );
  } 
}
