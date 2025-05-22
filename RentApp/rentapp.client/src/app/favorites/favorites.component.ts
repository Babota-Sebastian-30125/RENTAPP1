import { Component, OnInit } from '@angular/core';
import { FavoriteService } from '../services/favorites.service';
import { Favorite } from '../models/FavoriteModel';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-favorites',
  templateUrl: './favorites.component.html',
  styleUrls: ['./favorites.component.css'],
  standalone: true,
  imports: [CommonModule]
})
export class FavoritesComponent implements OnInit {
  favoriteProducts: Favorite[] = [];

  constructor(
    private favoriteService: FavoriteService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadFavoriteProducts();
  }

  loadFavoriteProducts(): void {
    this.favoriteService.getFavorites().subscribe(products => {
      this.favoriteProducts = products;
    });
  }

  toggleFavorite(productId: number): void {
    this.favoriteService.toggleFavorite(productId).subscribe(() => {
      this.loadFavoriteProducts();
    });
  }

  viewDetails(productId: number): void {
    this.router.navigate(['/products', productId]);
  }

  rentProduct(productId: string): void {
    this.router.navigate(['/rent', productId]);
  }
  getImageUrl(path: string): string {
    if (!path) return '';
    if (path.startsWith('/uploads')) {
      return 'https://localhost:7020' + path;
    }
    return path;
  }
}
