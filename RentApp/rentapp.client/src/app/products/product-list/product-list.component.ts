import { Component, OnInit } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/ProductModel';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { FavoriteService } from '../../services/favorites.service';
import { AuthService } from '../../services/auth.service';
import { forkJoin, of } from 'rxjs';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
})
export class ProductListComponent implements OnInit {
  products: Product[] = [];
  favorites: number[] = [];
  filteredProducts: Product[] = [];

  // Filtre și sortare
  searchQuery: string = '';
  selectedCategory: string = '';
  minPrice: number = 0;
  maxPrice: number = 1000;
  selectedLocation: string = '';
  sortBy: string = 'price';

  // Date din backend
  categories: string[] = [];
  locations: string[] = [];
  sortOptions: string[] = ['price', 'rating', 'newest']; // Opțiunile din backend

  showLoginPopup: boolean = false;
  isLoading: boolean = true;

  constructor(
    private productService: ProductService,
    private router: Router,
    private favoriteService: FavoriteService,
    public authService: AuthService
  ) { }

  ngOnInit(): void {
    // Încărcăm datele inițiale
    this.loadInitialData();
  }

  // Încarcă toate datele inițiale necesare pentru pagină
  loadInitialData(): void {
    this.isLoading = true;

    // Folosim forkJoin pentru a face toate cererile în paralel
    forkJoin({
      products: this.productService.getAll(),
      categories: this.productService.getCategories(), // Din backend
      locations: this.productService.getLocations(),   // Din backend
      favorites: this.authService.isLoggedIn() ? this.favoriteService.getFavorites() : of([])
    }).subscribe({
      next: (result) => {
        this.products = result.products;
        this.filteredProducts = result.products;
        this.categories = result.categories;
        this.locations = result.locations;
        this.favorites = Array.isArray(result.favorites) ?
          result.favorites.map(f => f.id) : [];
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Eroare la încărcarea datelor inițiale:', err);
        this.isLoading = false;
        // Încărcăm produsele chiar și în caz de eroare la alte date
        this.getProducts();
      }
    });
  }

  // Verifică dacă un produs este favorit
  isFavorite(productId: number): boolean {
    return this.favorites.includes(productId);
  }

  // Adaugă/elimină un produs din favorite
  toggleFavorite(productId: number) {
    if (!this.authService.isLoggedIn()) {
      this.showLoginPopup = true;
      return;
    }

    this.favoriteService.toggleFavorite(productId).subscribe({
      next: () => {
        if (this.isFavorite(productId)) {
          this.favorites = this.favorites.filter(id => id !== productId);
        } else {
          this.favorites.push(productId);
        }
      },
      error: (err) => {
        console.error('Eroare la toggle favorite:', err);
      }
    });
  }

  // Obține toate produsele
  getProducts(): void {
    this.isLoading = true;
    this.productService.getAll().subscribe({
      next: products => {
        this.products = products;
        this.filteredProducts = products;
        this.isLoading = false;
      },
      error: err => {
        console.error('Eroare la încărcarea produselor:', err);
        this.isLoading = false;
      }
    });
  }

  // Căutare după text
  searchProducts(): void {
    this.isLoading = true;
    this.productService.searchProducts(this.searchQuery).subscribe({
      next: products => {
        this.filteredProducts = products;
        this.isLoading = false;
      },
      error: err => {
        console.error('Eroare la căutarea produselor:', err);
        this.isLoading = false;
      }
    });
  }

  // Filtreză după categorie
  filterByCategory(): void {
    this.isLoading = true;
    if (this.selectedCategory) {
      this.productService.filterByCategory(this.selectedCategory).subscribe({
        next: products => {
          this.filteredProducts = products;
          this.isLoading = false;
        },
        error: err => {
          console.error('Eroare la filtrarea după categorie:', err);
          this.isLoading = false;
        }
      });
    } else {
      this.getProducts();
    }
  }

  // Filtrează după preț
  filterByPrice(): void {
    this.isLoading = true;
    this.productService.filterByPrice(this.minPrice, this.maxPrice).subscribe({
      next: products => {
        this.filteredProducts = products;
        this.isLoading = false;
      },
      error: err => {
        console.error('Eroare la filtrarea după preț:', err);
        this.isLoading = false;
      }
    });
  }

  // Filtrează după locație
  filterByLocation(): void {
    this.isLoading = true;
    this.productService.filterByLocation(this.selectedLocation).subscribe({
      next: products => {
        this.filteredProducts = products;
        this.isLoading = false;
      },
      error: err => {
        console.error('Eroare la filtrarea după locație:', err);
        this.isLoading = false;
      }
    });
  }

  // Sortează produsele
  sortProducts(): void {
    this.isLoading = true;
    this.productService.sortProducts(this.sortBy).subscribe({
      next: products => {
        this.filteredProducts = products;
        this.isLoading = false;
      },
      error: err => {
        console.error('Eroare la sortarea produselor:', err);
        this.isLoading = false;
      }
    });
  }

  // Aplică toate filtrele deodată
  applyAllFilters(): void {
    this.isLoading = true;
    this.productService.filterProducts(
      this.searchQuery,
      this.selectedCategory,
      this.minPrice,
      this.maxPrice,
      this.selectedLocation,
      this.sortBy
    ).subscribe({
      next: products => {
        this.filteredProducts = products;
        this.isLoading = false;
      },
      error: err => {
        console.error('Eroare la aplicarea filtrelor:', err);
        this.isLoading = false;
      }
    });
  }

  // Închiriază un produs
  rentProduct(id: string) {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/rent', id]);
    } else {
      this.showLoginPopup = true;
    }
  }

  // Navighează la pagina de login
  goToLogin() {
    this.showLoginPopup = false;
    this.router.navigate(['/login']);
  }

  // Închide popup-ul
  closePopup() {
    this.showLoginPopup = false;
  }

  // Navighează la detaliile produsului
  viewDetails(productId: number): void {
    this.router.navigate(['/products', productId]);
  }

  // Resetează toate filtrele
  resetFilters(): void {
    this.searchQuery = '';
    this.selectedCategory = '';
    this.minPrice = 0;
    this.maxPrice = 1000;
    this.selectedLocation = '';
    this.sortBy = 'price';
    this.getProducts();
  }
}
