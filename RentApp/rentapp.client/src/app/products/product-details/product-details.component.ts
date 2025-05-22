import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/ProductModel';
import { Review } from '../../models/ReviewModel';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product-details',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.css']
})
export class ProductDetailsComponent implements OnInit {
  product!: Product;
  reviews: Review[] = [];
  averageRating: number = 0; 
  isLoading = true;
  errorMessage = '';
  Math = Math;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService
  ) { }

  ngOnInit(): void {
    const productId = Number(this.route.snapshot.paramMap.get('id'));

    // Încarcă detaliile produsului
    this.productService.getProductDetails(productId).subscribe({
      next: (data) => {
        this.product = data.product;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Eroare la încărcarea detaliilor produsului.';
        this.isLoading = false;
      }
    });

    // Încarcă recenziile și calculează media
    this.productService.getReviews(productId).subscribe({
      next: (reviews) => {
        this.reviews = reviews;
        if (reviews.length > 0) {
          this.averageRating = reviews.reduce((sum, r) => sum + r.stars, 0) / reviews.length;
        } else {
          this.averageRating = 0;
        }
      },
      error: () => {
        this.errorMessage = 'Eroare la încărcarea recenziilor.';
      }
    });
  }

  goBack() {
    window.history.back();
  }
}
