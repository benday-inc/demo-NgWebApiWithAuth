import { Injectable } from '@angular/core';
import { User } from '../_model/user';
import { HttpClient } from '@angular/common/http';
import { GenericService } from './generic-service';

@Injectable({
  providedIn: 'root'
})
export class UserService extends GenericService<User> {

 constructor(
    http: HttpClient
  ) {
    super(http);
  }

  get endpoint(): string {
    return '/api/user';
  }
}
