import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Order } from '../model/order';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private host = "http://localhost:5156";

  constructor(private http: HttpClient) { }

  public getOrders(): Observable<Order[]> {
    return this.http.get<Order[]>(`${this.host}/api/order/all`);
  }
}
