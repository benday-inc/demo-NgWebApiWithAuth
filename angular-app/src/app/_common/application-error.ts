import { HttpErrorResponse } from '@angular/common/http';
import { ApplicationConstants } from './application-constants';

export class ApplicationError {
  errorNumber = 999;
  message: string = ApplicationConstants.emptyMessage;
  friendlyMessage: string= ApplicationConstants.emptyMessage;
  innerException: HttpErrorResponse | null = null;

  public initialize(message: string) {
    this.errorNumber = 999;
    this.message = message;
    this.friendlyMessage = message;
  }
}
