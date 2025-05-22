import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ReviewService {
  private apiUrl = 'https://localhost:7020/api/reviews';

  constructor(private http: HttpClient) { }

  addReview(productId: number, review: { stars: number, comment: string }): Observable<any> {
    const headers = { Authorization: 'Bearer ' + localStorage.getItem('token') };
    return this.http.post(`${this.apiUrl}/${productId}`, review, { headers });
  }
}
