import { HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { ApplicationError } from '../_common/application-error';

export class CommonUtilities {
  public static handleHttpError<T>(error: HttpErrorResponse): Observable<T> {
    if (error !== null) {
      console.log(`handleHttpError(): ${JSON.stringify(error)}`);

      const dataError = new ApplicationError();
      dataError.errorNumber = error.status;
      dataError.message = error.statusText;
      dataError.innerException = error;

      if (error.status === 401) {
        dataError.friendlyMessage = 'Error retrieving data: not logged in';
      } else if (error.status === 403) {
        dataError.friendlyMessage = 'Error retrieving data: forbidden';
      } else {
        dataError.friendlyMessage = `Error retrieving data: ${error.statusText}`;
      }

      return throwError(dataError);
    } else {
      const dataError = new ApplicationError();
      dataError.errorNumber = -1;
      dataError.message = 'An error happened by the error was null.';
      dataError.friendlyMessage = 'An error happened by the error was null..';
      dataError.innerException = error;
      return throwError(dataError);
    }
  }

  public static formatErrorMessageFromAny(error: any): string {
    if (!error) {
      return 'Error was undefined';
    } else if(error === null) {
      return 'Error was null';
    } else if (!(error instanceof ApplicationError)) 
    {
      return JSON.stringify(error);
    } else {
      return this.formatErrorMessage(error as ApplicationError);
    }
  }

  public static formatErrorMessage(error: ApplicationError): string {
    if (!error) {
      return 'Application error was null or undefined';
    } else {
      console.log(error);
      
      if (!error.innerException || !error.innerException.error) {
        return `An error occurred.  ${error.errorNumber} - ${error.message} - ${error.friendlyMessage}`;
      } else {

        return `An error occurred.  ${error.friendlyMessage}`;
      }
    }
  }
}
