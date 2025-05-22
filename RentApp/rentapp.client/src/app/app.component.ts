import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from './services/auth.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  userName: string | null = '';
  userEmail: string | null = '';
  userTelephone: string | null = '';
  message: string = '';
  showDetails: boolean = false;
  showChangePassword: boolean = false;
  oldPassword: string = '';
  newPassword: string = '';

  constructor(
    public authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.userName = this.authService.getUserName();
    this.userEmail = localStorage.getItem('email');
    this.userTelephone = localStorage.getItem('telephoneNumber');
    this.authService.userName$.subscribe(name => this.userName = name);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  goToMyRentals(): void {
    this.router.navigate(['/rentals/my']);
  }

  changePassword(): void {
    this.authService.changePassword(this.oldPassword, this.newPassword)
      .subscribe({
        next: resp => {
          this.message = resp.Message;
          this.oldPassword = '';
          this.newPassword = '';
          this.showChangePassword = false;
        },
        error: err => {
          if (err.status === 400) {
            this.message = 'Parola veche este incorectă.';
          } else if (err.status === 401) {
            this.message = 'Autentificare eșuată. Token invalid.';
          } else if (err.status === 404) {
            this.message = 'Utilizatorul nu a fost găsit.';
          } else {
            this.message = 'Eroare la schimbarea parolei. Încearcă din nou.';
          }
        }
      });
  }
}
