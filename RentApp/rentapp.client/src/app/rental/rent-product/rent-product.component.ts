import { Component, Input, OnInit } from '@angular/core';
import { RentalService } from '../../services/rental.service';
import { RentalRequest } from '../../models/RentalModel';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-rent-product',
  templateUrl: './rent-product.component.html',
  styleUrls: ['./rent-product.component.css'],
  standalone: true,
  imports: [FormsModule, CommonModule]
})
export class RentProductComponent implements OnInit {
  @Input() productId!: number;
  startDate!: string;
  endDate!: string;
  pricePerDay: number = 0; 
  message = '';
  showPreview = false;
  totalPrice = 0;

  constructor(private rentalService: RentalService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.productId = +idParam;

    
      this.rentalService.getProductDetails(this.productId).subscribe({
        next: (product) => {
          this.pricePerDay = product.pricePerDay; // Setează prețul din backend
        },
        error: (err) => {
          this.message = err.error?.message || 'Eroare la încărcarea detaliilor produsului.';
        }
      });
    }
  }


  calculateTotal(): void {
    if (this.startDate && this.endDate) {
      const start = new Date(this.startDate);
      const end = new Date(this.endDate);
      const days = (end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24);

      if (days > 0) {
        this.totalPrice = days * this.pricePerDay;
        this.showPreview = true;
        this.message = '';
      } else {
        this.showPreview = false;
        this.message = 'Data de sfârșit trebuie să fie după data de început.';
      }
    }
  }

  rentProduct(): void {
    const request: RentalRequest = {
      productId: this.productId,
      startDate: this.startDate,
      endDate: this.endDate
    };

    this.rentalService.rentProduct(request).subscribe({
      next: res => {
        this.message = 'Închirierea a fost finalizată cu succes!';
        this.showPreview = false;
      },
      error: err => {
        this.message = err.error?.message || 'Eroare la închiriere.';
      }
    });
  }
}
