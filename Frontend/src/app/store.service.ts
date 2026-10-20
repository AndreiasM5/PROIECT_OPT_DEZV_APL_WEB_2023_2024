import { inject, Injectable, signal } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { defer, of, tap } from 'rxjs';
import { environment } from '../environment';
import { BrowserStore } from './browser-store';
import type { Product, Order, Session, Draft } from './models';
@Injectable({providedIn:'root'})
export class StoreService {
  private http=inject(HttpClient);
  private document=inject(DOCUMENT);
  readonly demo=environment.browserDemo;
  session=signal<Session|null>(this.restore());
  private store?: BrowserStore;
  private restore(): Session|null {
    if(environment.browserDemo)return {token:'',email:'vizitator@demo.local',roles:['User'],expiration:'9999-01-01T00:00:00Z'};
    try { const s=JSON.parse(this.document.defaultView?.sessionStorage.getItem('dotnet.store.session')??'null'); return s && typeof s.token==='string' && s.token && Array.isArray(s.roles) && Date.parse(s.expiration)>Date.now() ? s : null; }catch{return null;}
  }
  private options() { return {headers:new HttpHeaders({Authorization:'Bearer '+(this.session()?.token??'')})}; }
  private local<T>(fn:(s:BrowserStore)=>T) { return defer(()=>{ if(!this.store) { try { const storage=this.document.defaultView?.localStorage;if(!storage)throw new Error();this.store=new BrowserStore(storage); }catch{throw new Error('Stocarea locală nu este disponibilă.');} }return of(fn(this.store));}); }
  login(email:string,password:string) { return this.http.post<Session>('/api/login',{email,password}).pipe(tap(s=>{this.document.defaultView?.sessionStorage.setItem('dotnet.store.session',JSON.stringify(s));this.session.set(s);})); }
  register(email:string,password:string) {return this.http.post('/api/register',{email,password});}
  logout() {this.document.defaultView?.sessionStorage.removeItem('dotnet.store.session');this.session.set(null);}
  switchRole() {if(this.demo)this.session.update(s=>s?{...s,roles:s.roles.includes('Admin')?['User']:['Admin']}:s);}
  products() {return this.demo?this.local(s=>s.products()):this.http.get<Product[]>('/api/product/all',this.options());}
  orders() {return this.demo?this.local(s=>s.orders()):this.http.get<Order[]>('/api/order/all',this.options());}
  saveProduct(d:Draft,id:number|null) {if(this.demo)return this.local(s=>s.saveProduct(d,id));return id===null?this.http.post<Product>('/api/product',d,this.options()):this.http.put<Product>('/api/product/'+id,d,this.options());}
  deleteProduct(id:number) {return this.demo?this.local(s=>s.deleteProduct(id)):this.http.delete<void>('/api/product/'+id,this.options());}
  saveOrder(ids:number[],id:number|null) {if(this.demo)return this.local(s=>s.saveOrder(ids,id));return id===null?this.http.post<Order>('/api/order',{productsIds:ids},this.options()):this.http.put<Order>('/api/order/'+id,{productsIds:ids},this.options());}
  deleteOrder(id:number) {return this.demo?this.local(s=>s.deleteOrder(id)):this.http.delete<void>('/api/order/'+id,this.options());}
}
