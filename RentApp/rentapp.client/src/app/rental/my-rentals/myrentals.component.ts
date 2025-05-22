import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RentalService } from '../../services/rental.service';
import { ReviewService } from '../../services/review.service';
import { ReviewComponent } from '../../reviews/review.component';

@Component({
  selector: 'app-my-rentals',
  standalone: true,
  imports: [CommonModule, ReviewComponent],
  templateUrl: './myrentals.component.html',
  styleUrls: ['./myrentals.component.css']
})
export class MyRentalsComponent implements OnInit {
  rentals: any[] = [];
  message = '';
  openReviewForRentalId: number | null = null;
  loading: boolean = false;

  constructor(
    private rentalService: RentalService,
    private reviewService: ReviewService
  ) { }

  ngOnInit(): void {
    this.loadRentals();
  }

  loadRentals(): void {
    this.rentalService.getMyRentals().subscribe({
      next: (data) => {
        this.rentals = data.map(r => ({
          ...r,
          status: this.getRentalStatus(r.startDate, r.endDate)
        }));
      },
      error: () => {
        this.message = 'Eroare la încărcarea închirierilor.';
      }
    });
  }

  getRentalStatus(startDateStr: string, endDateStr: string): string {
    const today = new Date();
    const start = new Date(startDateStr);
    const end = new Date(endDateStr);
    if (today < start) return 'Inactiv';
    if (today > end) return 'Finalizat';
    return 'Activ';
  }

  showReviewPopup(rentalId: number) {
    this.openReviewForRentalId = rentalId;
  }

  closeReviewPopup() {
    this.openReviewForRentalId = null;
  }

  handleReviewSubmit(event: { rating: number; comment: string }, rental: any) {
    this.loading = true;
    const review = { stars: event.rating, comment: event.comment };
    this.reviewService.addReview(rental.productId, review).subscribe({
      next: () => {
        this.message = 'Recenzia a fost adăugată cu succes!';
        this.closeReviewPopup();
        this.loading = false;
        setTimeout(() => this.message = '', 3000);
      },
      error: () => {
        this.message = 'Eroare la adăugarea recenziei.';
        this.loading = false;
        setTimeout(() => this.message = '', 3000);
      }
    });
  }

  confirmDelete(rentalId: number) {
    if (confirm('Ești sigur că vrei să ștergi această închiriere?')) {
      this.loading = true;
      this.rentalService.cancelRental(rentalId).subscribe({
        next: () => {
          this.message = 'Închirierea a fost ștearsă cu succes!';
          this.loadRentals();
          this.loading = false;
          setTimeout(() => this.message = '', 3000);
        },
        error: () => {
          this.message = 'Eroare la ștergerea închirierii.';
          this.loading = false;
          setTimeout(() => this.message = '', 3000);
        }
      });
    }
  }
}
