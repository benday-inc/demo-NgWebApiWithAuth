import { Component, OnInit } from '@angular/core';
import { UserService } from '../../../../_services/user.service';
import { User } from '../../../../_model/user';
import { ApplicationConstants } from '../../../../_common/application-constants';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-list',
  imports: [CommonModule],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.css'
})
export class UserListComponent implements OnInit {
  public isLoading = false;
  public isLoaded = false;
  public message: string = ApplicationConstants.emptyMessage;
  public items: User[] = [];

  constructor(
    private service: UserService,
    private router: Router
  ) { }

  ngOnInit() {
    const ownerId = ApplicationConstants.ownerIdForSystem

    this.load(ownerId);
  }

  private load(ownerId: string) {

    this.isLoading = true;
    this.isLoaded = false;

    this.service.getListByOwnerId(ownerId).subscribe({
      next: (result) => {
        this.items = result;
        this.isLoading = false;
        this.isLoaded = true;
        this.afterLoad();
      },
      error: (error) => {
        this.isLoading = false;
        this.isLoaded = true;
        console.error(error);
        this.message = error.message;
      }
    });
  }

  rowClicked(event: MouseEvent, item: User) {
    const url = `/user/detail/${item.ownerId}/${item.id}`;

    console.log('Navigating to ' + url);

    this.router.navigate([url]);
  }

  afterLoad() {
    console.log('Loaded ' + this.items.length + ' items');    
  }

  createNew() {
    let ownerId = ApplicationConstants.ownerIdForSystem
    if (ownerId == '' || ownerId == null || ownerId == undefined) {

    }
    else {
      this.router.navigate([`User/new/${ownerId}`]);
    }
  }

  refresh() {
    let ownerId = ApplicationConstants.ownerIdForSystem
    
    if (ownerId == '' || ownerId == null || ownerId == undefined) {

    }
    else {
      this.load(ownerId);
    }
  }

}
