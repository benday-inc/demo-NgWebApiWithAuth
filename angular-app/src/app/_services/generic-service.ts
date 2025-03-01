import { HttpClient } from "@angular/common/http";
import { Observable, catchError } from "rxjs";
import { CommonUtilities } from "../_common/common-utilities";


export abstract class GenericService<T> {

  constructor(
    protected http: HttpClient) { }

  abstract get endpoint(): string;

  getListByOwnerId(ownerId: string): Observable<T[]> {
    if (ownerId === '' || ownerId === null || ownerId === undefined) {
      throw new Error('ownerId is required.');
    }

    const url = `${this.endpoint}/GetAllByOwnerId/${ownerId}`;

    console.log(`getListByOwnerId(): url=${url}`);

    return this.http.get<T[]>(url).pipe(
      catchError(err => CommonUtilities.handleHttpError<T[]>(err))
    );
  }

  get(ownerId: string, id: string): Observable<T | null> {
    const url = `${this.endpoint}/GetByIdAndOwnerId/${ownerId}/${id}`;

    console.log(`get(): url=${url}`);

    return this.http.get<T | null>(
      url).pipe(
        catchError(err => CommonUtilities.handleHttpError<T | null>(err))
      );
  }
}
