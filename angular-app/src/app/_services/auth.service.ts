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

  public login(user: { email: string; password: string }) {
    let data = new AuthRequestResponse();
    data.email = user.email;
    data.password = user.password;
    return this.http.post<AuthRequestResponse>(`${this.endpoint}/login`, data).pipe(
      tap(response => {
        localStorage.setItem('token', response.token); // Store the token
      }),
      catchError(err => CommonUtilities.handleHttpError<AuthRequestResponse>(err))
    );
  } 
}
