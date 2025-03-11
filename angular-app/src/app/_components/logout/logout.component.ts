import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../_services/auth.service';

@Component({
  selector: 'app-logout',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './logout.component.html',
  styleUrl: './logout.component.css'
})
export class LogoutComponent implements OnInit {
  constructor(
    private service: AuthService,
    private router: Router) {    
  }

  ngOnInit() {
    this.logout();
  }

  logout() {
    this.service.logout().subscribe({
      next: (response) => {
        console.log('Logout successful');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error(err);
      }
    });
  }
}
