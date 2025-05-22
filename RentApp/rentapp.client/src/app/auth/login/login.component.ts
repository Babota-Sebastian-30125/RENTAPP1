import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  email = '';
  password = '';
  error = '';
  success = '';

  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/products']);
    }
  }

  onSubmit(): void {
    this.error = '';
    this.success = '';

    this.authService.login(this.email, this.password).subscribe({
      next: (response) => {
     
        localStorage.setItem('token', response.token);
        localStorage.setItem('name', response.name);
        localStorage.setItem('email', response.email);
        localStorage.setItem('telephoneNumber', response.telephoneNumber);


        this.success = 'Conectare realizata cu succes!';

        setTimeout(() => {
          this.router.navigate(['/products']);
        }, 1000);
      },
      error: (error) => {
        //mesaju din backend
        this.error = error.message || 'Parola sau e-mail invalid!';
      }
    });
  }
}
