import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Product } from '../model/product';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private host = "http://localhost:5156";

  constructor(private http: HttpClient) { }

  public getProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.host}/api/product/all`);
  }

  public getProduct(productId : number): Observable<Product>{
    return this.http.get<Product>(`${this.host}/api/product/${productId}`)
  }
}
