import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { RentalRequest, RentalResponse } from '../models/RentalModel';

@Injectable({
  providedIn: 'root'
})
export class RentalService {
  private apiUrl = '/api/rentals';

  constructor(private http: HttpClient) { }

  rentProduct(data: RentalRequest): Observable<RentalResponse> {
    return this.http.post<RentalResponse>(this.apiUrl, data);
  }

  getMyRentals(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/my`);
  }

  cancelRental(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  getProductDetails(productId: number): Observable<{ id: number; name: string; pricePerDay: number; description: string; available: boolean }> {
    return this.http.get<{ id: number; name: string; pricePerDay: number; description: string; available: boolean }>(`${this.apiUrl}/product/${productId}`);
  }

  
  getRequestsForProduct(productId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/product/${productId}`);
  }
}
