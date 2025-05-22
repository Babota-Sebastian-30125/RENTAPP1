import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-product-form',
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.css'],
  imports: [CommonModule, RouterModule, ReactiveFormsModule]
})
export class ProductFormComponent implements OnInit {
  form: FormGroup;
  selectedFile: File | null = null;
  categories: string[] = [];
  locations: string[] = [];

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      category: ['', Validators.required],
      location: ['', Validators.required],
      description: ['', Validators.required],
      pricePerDay: [0, [Validators.required, Validators.min(0.01)]],
      available: [true]
    });
  }

  ngOnInit(): void {
    this.productService.getCategories().subscribe({
      next: (cats) => this.categories = cats,
      error: () => this.categories = []
    });

    this.productService.getLocations().subscribe({
      next: (locs) => this.locations = locs,
      error: () => this.locations = []
    });
    this.route.queryParams.subscribe(params => {
      if (params['edit']) {
        const productId = +params['edit'];
        this.productService.getById(productId).subscribe(product => {
          this.form.patchValue(product);
        });
      }
    });
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
    }
  }

  submit() {
    const product = this.form.value;
    this.productService.create(product, this.selectedFile).subscribe({
      next: () => this.router.navigate(['/products']),
      error: (err) => {
        console.error('Eroare la salvare produs:', err);
        alert('A apărut o eroare la salvare!');
      }
    });
  }
}
