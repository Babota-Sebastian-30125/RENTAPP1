
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReviewService } from '../services/review.service';
import { Review } from './review.model';

@Component({
  selector: 'app-review',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './review.component.html',
  styleUrls: ['./review.component.css']
})
export class ReviewComponent {
  @Input() productId!: number;
  @Input() externalMessage: string = '';
  @Output() submitted = new EventEmitter<Review>();
  @Output() canceled = new EventEmitter<void>();

  rating: number = 0;
  comment: string = '';

  constructor(private reviewService: ReviewService) { }

  setRating(rating: number) {
    this.rating = rating;
  }

  submitReview() {
    if (this.rating && this.comment) {
      const review: Review = {
        productId: this.productId,
        rating: this.rating,
        comment: this.comment
      };
      this.submitted.emit(review);
    }
  }

  cancel() {
    this.canceled.emit();
  }
}
