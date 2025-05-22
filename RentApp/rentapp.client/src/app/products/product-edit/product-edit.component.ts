import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-edit-product',
  standalone: true,
  templateUrl: './product-edit.component.html',
  styleUrls: ['./product-edit.component.css'],
  imports: [CommonModule, ReactiveFormsModule]
})
export class EditProductComponent implements OnInit {
  productForm!: FormGroup;
  productId!: number;
  selectedFile: File | null = null;
  categories: string[] = [];
  locations: string[] = [];
  message = '';

  constructor(
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private productService: ProductService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.productId = Number(this.route.snapshot.paramMap.get('id'));
    this.productService.getCategories().subscribe(cats => this.categories = cats);
    this.productService.getLocations().subscribe(locs => this.locations = locs);

    // Inițializează formularul gol ca să nu dea erori în template
    this.productForm = this.fb.group({
      name: ['', Validators.required],
      category: ['', Validators.required],
      location: ['', Validators.required],
      description: ['', Validators.required],
      pricePerDay: [null, [Validators.required, Validators.min(1)]],
      available: [false]
    });

    // Populează formularul cu datele reale când vin din backend
    this.productService.getById(this.productId).subscribe(product => {
      this.productForm.patchValue({
        name: product.name,
        category: product.category,
        location: product.location,
        description: product.description,
        pricePerDay: product.pricePerDay,
        available: product.available
      });
    });
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
    }
  }

  submit() {
    if (this.productForm.invalid) return;
    const product = this.productForm.value;
    this.productService.update(this.productId, product, this.selectedFile).subscribe({
      next: () => {
        this.message = 'Produs modificat cu succes!';
        setTimeout(() => this.router.navigate(['/my-products']), 1200);
      },
      error: () => {
        this.message = 'Eroare la modificare!';
      }
    });
  }
}
