import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product } from "../models/ProductModel";
import { map } from 'rxjs/operators';
import { Review } from '../models/ReviewModel';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = '/api/products';
  private reviewsUrl = '/api/reviews';

  constructor(private http: HttpClient) { }

  getCategories(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/categories`);
  }

  getLocations(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/locations`);
  }

  create(product: Product, imageFile: File | null): Observable<Product> {
    const formData = new FormData();
    formData.append('name', product.name);
    formData.append('category', product.category);
    formData.append('location', product.location);
    formData.append('description', product.description);
    formData.append('pricePerDay', product.pricePerDay.toString());
    formData.append('available', product.available.toString());
    if (imageFile) {
      formData.append('imageFile', imageFile, imageFile.name);
    }
    return this.http.post<Product>(`${this.apiUrl}`, formData, {
      headers: new HttpHeaders({
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      })
    });
  }

  getAll(): Observable<Product[]> {
    return this.http.get<any>(this.apiUrl).pipe(
      map(response => response.products)
    );
  }

  // *** METODA MODIFICATĂ: Preia doar produsul din răspunsul backend ***
  getById(id: number): Observable<Product> {
    return this.http.get<{ message: string, product: Product }>(`${this.apiUrl}/${id}`).pipe(
      map(response => response.product)
    );
  }

  searchProducts(query: string): Observable<Product[]> {
    return this.http.get<any>(`${this.apiUrl}?search=${query}`).pipe(
      map(response => response.products)
    );
  }

  filterByPrice(minPrice: number, maxPrice: number): Observable<Product[]> {
    return this.http.get<any>(`${this.apiUrl}?minPrice=${minPrice}&maxPrice=${maxPrice}`).pipe(
      map(response => response.products)
    );
  }

  filterByCategory(category: string): Observable<Product[]> {
    return this.http.get<any>(`${this.apiUrl}?category=${category}`).pipe(
      map(response => response.products)
    );
  }

  filterByLocation(location: string): Observable<Product[]> {
    return this.http.get<any>(`${this.apiUrl}?location=${location}`).pipe(
      map(response => response.products)
    );
  }

  sortProducts(sortBy: string): Observable<Product[]> {
    return this.http.get<any>(`${this.apiUrl}?sortBy=${sortBy}`).pipe(
      map(response => response.products)
    );
  }

  filterProducts(
    search: string = '',
    category: string = '',
    minPrice: number = 0,
    maxPrice: number = 0,
    location: string = '',
    sortBy: string = 'price'
  ): Observable<Product[]> {
    let url = `${this.apiUrl}?`;
    if (search) url += `search=${search}&`;
    if (category) url += `category=${category}&`;
    if (minPrice > 0) url += `minPrice=${minPrice}&`;
    if (maxPrice > 0) url += `maxPrice=${maxPrice}&`;
    if (location) url += `location=${location}&`;
    if (sortBy) url += `sortBy=${sortBy}`;
    return this.http.get<any>(url).pipe(
      map(response => response.products)
    );
  }

  getProductDetails(id: number): Observable<{ product: Product }> {
    return this.http.get<{ product: Product }>(`${this.apiUrl}/${id}`);
  }

  getReviews(productId: number): Observable<Review[]> {
    return this.http.get<Review[]>(`${this.reviewsUrl}/product/${productId}`);
  }

  getAverageRating(productId: number): Observable<{ average: number }> {
    return this.http.get<{ average: number }>(`${this.reviewsUrl}/product/${productId}/average`);
  }

  delete(productId: number): Observable<any> {
    return this.http.delete(`/api/products/${productId}`);
  }

  update(id: number, product: Product, imageFile: File | null): Observable<any> {
    const formData = new FormData();
    formData.append('name', product.name);
    formData.append('category', product.category);
    formData.append('location', product.location);
    formData.append('description', product.description);
    formData.append('pricePerDay', product.pricePerDay.toString());
    formData.append('available', product.available.toString());
    if (imageFile) {
      formData.append('imageFile', imageFile, imageFile.name);
    }
    return this.http.put<any>(`/api/products/${id}`, formData, {
      headers: new HttpHeaders({
        'Authorization': `Bearer ${localStorage.getItem('token')}`
      })
    });
  }
}
