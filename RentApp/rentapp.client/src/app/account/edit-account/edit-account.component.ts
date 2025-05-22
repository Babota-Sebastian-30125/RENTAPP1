import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms'; 

@Component({
  selector: 'app-edit-account',
  templateUrl: './edit-account.component.html',
  styleUrls: ['./edit-account.component.css'],
  imports: [FormsModule]
})
export class EditAccountComponent implements OnInit {
  userName: string | null = '';
  userEmail: string | null = '';
  userTelephone: string | null = '';

  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
    } else {
      this.userName = localStorage.getItem('name');
      this.userEmail = localStorage.getItem('email');
      this.userTelephone = localStorage.getItem('telephoneNumber');
    }
  }

  onSubmit(): void {
    console.log('Cont actualizat', this.userName, this.userEmail, this.userTelephone);
  }
}
