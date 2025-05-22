import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  name = '';
  email = '';
  telephoneNumber = '';
  password = '';
  error = '';
  success = '';

  constructor(private authService: AuthService, private router: Router) { }

  onSubmit(): void {
    this.error = '';
    this.success = '';

    this.authService.register(this.name, this.email, this.telephoneNumber, this.password).subscribe({
      next: () => {
        this.success = 'Inregistrare realizata cu succes!';
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 1500);
      },
      error: (error) => {
      
        this.error = error.message || 'Inregistrare esuata. Te rugam sa incerci din nou!';
      }
    });
  }
}
