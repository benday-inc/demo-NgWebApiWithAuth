import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../_services/auth.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  imports: [ ReactiveFormsModule, CommonModule ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {

  private formBuilder = inject(FormBuilder);
  
  message: string = '';

  loginForm = this.formBuilder.nonNullable.group({
    username: ['', Validators.required],
    password: ['', Validators.required]
  });

  constructor(
    private service: AuthService,
    private router: Router) {    
  }

  login() {
    if (this.loginForm.valid) {
      this.message = '';
      // Handle the login logic here
      console.log('Form submitted for login', this.loginForm.value);

      const username = this.loginForm.controls.username.value;
      const password = this.loginForm.controls.password.value;

      this.service.login(username, password).subscribe({
        next: (response) => {
          console.log('Login successful');
          this.message = 'Login successful';
          console.log(response);
          this.router.navigate(['/']);
        },
        error: (err) => {
          console.error(err);
          this.message = 'Login failed';
        }
      });
    }     
  }

  register() {
    if (this.loginForm.valid) {
      // Handle the registration logic here
      this.message = '';
      console.log('Form submitted for register', this.loginForm.value);

      const username = this.loginForm.controls.username.value;
      const password = this.loginForm.controls.password.value;

      this.service.register(username, password).subscribe({
        next: (response) => {
          console.log('Register successful');
          this.message = 'Register successful';
          console.log(response);
          this.router.navigate(['/']);
        },
        error: (err) => {
          console.error(err);
          this.message = 'Register failed';
        }
      });
    }     
  }
}
