import { Component, OnInit } from '@angular/core';
import { Product } from '../model/product';
import { ProductService } from '../service/product.service';
import { OrderService } from '../service/order.service';
import { Order } from '../model/order';

@Component({
  selector: 'app-orders',
  templateUrl: './orders.component.html',
  styleUrl: './orders.component.css'
})
export class OrdersComponent implements OnInit{

  public products: Product[] = [];
  public orders: Order[] = [];

  constructor (private productService: ProductService, private orderService: OrderService) {}

  ngOnInit(): void {
    this.productService.getProducts().subscribe(
      (data: Product[]) => {
        this.products = data;
      },
      (error) => {
        console.error('Error loading products:', error);
      }
    );

    this.orderService.getOrders().subscribe(
      (data: Order[]) => {
        this.orders = data;
      },
      (error) => {
        console.error('Error loading orders:', error);
      }
    );
  }
}
