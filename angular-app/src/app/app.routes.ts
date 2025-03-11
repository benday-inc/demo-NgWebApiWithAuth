import { Routes } from '@angular/router';
import { HomeComponent } from './_components/home/home.component';
import { LoginComponent } from './_components/login/login.component';
import { SecuredPageComponent } from './_components/secured-page/secured-page.component';
import { UserListComponent } from './_components/user/list/user-list/user-list.component';
import { UserDetailComponent } from './_components/user/detail/user-detail/user-detail.component';
import { LogoutComponent } from './_components/logout/logout.component';

export const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'logout', component: LogoutComponent },
  { path: 'secured', component: SecuredPageComponent },
  { path: 'user/list', component: UserListComponent },
  { path: 'user/detail/:ownerId/:id', component: UserDetailComponent },
];
