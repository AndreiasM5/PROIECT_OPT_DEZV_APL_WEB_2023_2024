export class Order {
    orderId: number;
    totalAmount: number;
    productsIds: number[];
  
    constructor(orderId: number, totalAmount: number, productsIds: number[]) {
      this.orderId = orderId;
      this.totalAmount = totalAmount;
      this.productsIds = productsIds;
    }
  }
  