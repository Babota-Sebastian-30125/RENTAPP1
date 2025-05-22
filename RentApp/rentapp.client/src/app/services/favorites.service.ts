import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Favorite } from '../models/FavoriteModel';

@Injectable({
  providedIn: 'root'
})
export class FavoriteService {
  private apiUrl = 'https://localhost:7020/api/favorites';

  constructor(private http: HttpClient) { }

  getFavorites(): Observable<Favorite[]> {
    return this.http.get<Favorite[]>(this.apiUrl);
  }

  toggleFavorite(productId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/${productId}`, {});
  }
}
