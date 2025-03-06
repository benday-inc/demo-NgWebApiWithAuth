import { Injectable } from '@angular/core';
import { GenericService } from './generic-service';
import { GetMessageResponse } from '../_models/get-message-response';
import { catchError, Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { CommonUtilities } from '../_common/common-utilities';

@Injectable({
  providedIn: 'root'
})
export class SecuredPageService extends GenericService<GetMessageResponse> {

  constructor(
    http: HttpClient
  ) {
    super(http);
  }

  get endpoint(): string {
    return '/api/secured';
  }

  getProtectedData(): Observable<GetMessageResponse> {
    const url = `${this.endpoint}/protected`;

    return this.http.get<GetMessageResponse>(
      url).pipe(
        catchError(err => CommonUtilities.handleHttpError<GetMessageResponse>(err))
      );
  }

}
