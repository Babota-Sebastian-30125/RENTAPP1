import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup, FormControl, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-review',
  templateUrl: './review.component.html',
  styleUrls: ['./review.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule]
})
export class ReviewComponent {
  @Input() productId!: number;
  @Input() externalMessage: string = '';
  @Output() submitted = new EventEmitter<{ rating: number; comment: string }>();
  @Output() canceled = new EventEmitter<void>();

  reviewForm = new FormGroup({
    comment: new FormControl('')
  });
  stars = [1, 2, 3, 4, 5];
  selectedRating = 0;
  errorMessage = '';

  selectStar(star: number): void {
    this.selectedRating = star;
  }

  submitReview(): void {
    this.errorMessage = '';
    if (this.selectedRating > 0) {
      this.submitted.emit({
        rating: this.selectedRating,
        comment: this.commentControl.value || ''
      });
    } else {
      this.errorMessage = 'Te rugăm să selectezi un număr de stele.';
    }
  }

  cancel(): void {
    this.canceled.emit();
  }

  get commentControl(): FormControl<string> {
    return this.reviewForm.get('comment') as FormControl<string>;
  }
}
