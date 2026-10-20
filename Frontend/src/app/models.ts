export interface Product { productId: number; name: string; price: number; }
export interface Order { orderId: number; totalAmount: number; productsIds: number[]; }
export interface Session { token: string; expiration: string; email: string; roles: string[]; }
export type Draft = Omit<Product, 'productId'>;
