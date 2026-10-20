import { Component, Directive, ElementRef, AfterViewInit, OnDestroy, HostListener, inject, signal, computed, effect } from '@angular/core';
import { DOCUMENT, DecimalPipe } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { finalize, forkJoin } from 'rxjs';
import { StoreService } from './store.service';
import type { Product, Order, Draft } from './models';
@Directive({selector: '[trapFocus]', standalone: true})
class TrapFocus implements AfterViewInit, OnDestroy {
  private element = inject<ElementRef<HTMLElement>>(ElementRef);
  private document = inject(DOCUMENT);
  private previous = this.document.activeElement as HTMLElement | null;
  ngAfterViewInit() {
    const host = this.element.nativeElement;
    (host.querySelector<HTMLElement>('input') ?? host.querySelector<HTMLElement>('button'))?.focus();
  }
  @HostListener('keydown', ['$event'])
  onKey(event: KeyboardEvent) {
    if(event.key !== 'Tab') return;
    const nodes = Array.from(this.element.nativeElement.querySelectorAll<HTMLElement>('button:not(:disabled),input:not(:disabled),select:not(:disabled),textarea:not(:disabled),a[href]')).filter(node => node.getClientRects().length > 0);
    const first = nodes[0], last = nodes[nodes.length - 1];
    if(!first) { event.preventDefault(); return; }
    if(event.shiftKey && this.document.activeElement === first) { event.preventDefault(); last.focus(); }
    else if(!event.shiftKey && this.document.activeElement === last) { event.preventDefault(); first.focus(); }
  }
  ngOnDestroy() { this.previous?.focus(); }
}

@Component({selector:'app-root',standalone:true,imports:[FormsModule,DecimalPipe,TrapFocus],templateUrl:'./app.component.html'})
export class AppComponent {
  api=inject(StoreService);
  products=signal<Product[]>([]); orders=signal<Order[]>([]); cart=signal<number[]>([]);
  tab=signal('catalog'); search=signal(''); loading=signal(false); busy=signal(false);
  error=signal('');notice=signal(''); editor=signal(false); editingProduct=signal<number|null>(null); editingOrder=signal<number|null>(null);
  confirm=signal<{kind:'product'|'order';id:number;name:string}|null>(null);
  draft:Draft={name:'',price:0}; email='customer@example.com';password='DemoCustomer123!';registering=false;
  admin=computed(()=>this.api.session()?.roles.includes('Admin')??false);
  filtered=computed(()=>this.products().filter(p=>p.name.toLocaleLowerCase('ro').includes(this.search().toLocaleLowerCase('ro'))));
  chosen=computed(()=>this.products().filter(p=>this.cart().includes(p.productId)));
  total=computed(()=>this.chosen().reduce((sum,p)=>sum+p.price,0));
  constructor(){effect(()=>{if(this.api.session())this.load();else{this.products.set([]);this.orders.set([]);this.cart.set([]);}});}
  load(){this.loading.set(true);this.error.set('');forkJoin({products:this.api.products(),orders:this.api.orders()}).pipe(finalize(()=>this.loading.set(false))).subscribe({next:r=>{this.products.set(r.products);this.orders.set(r.orders);this.cart.update(ids=>ids.filter(id=>r.products.some(p=>p.productId===id)));},error:e=>this.fail(e)});}
  auth(){this.busy.set(true);this.error.set('');const request=this.registering?this.api.register(this.email,this.password):this.api.login(this.email,this.password);request.pipe(finalize(()=>this.busy.set(false))).subscribe({next:()=>{if(this.registering){this.registering=false;this.notice.set('Cont creat. Acum te poți autentifica.');}},error:e=>this.fail(e)});}
  quick(role:'admin'|'customer'){this.email=role+'@example.com';this.password=role==='admin'?'DemoAdmin123!':'DemoCustomer123!';this.registering=false;this.auth();}
  toggle(id:number){this.cart.update(ids=>ids.includes(id)?ids.filter(x=>x!==id):[...ids,id]);}
  product(p?:Product){this.editingProduct.set(p?.productId??null);this.draft={name:p?.name??'',price:p?.price??0};this.error.set('');this.editor.set(true);}
  saveProduct(form:NgForm){if(form.invalid||this.busy())return;this.busy.set(true);this.error.set('');this.api.saveProduct(this.draft,this.editingProduct()).pipe(finalize(()=>this.busy.set(false))).subscribe({next:()=>{this.editor.set(false);this.notice.set('Produsul a fost salvat.');this.load();},error:e=>this.fail(e)});}
  placeOrder(){if(!this.cart().length||this.busy())return;this.busy.set(true);this.error.set('');this.api.saveOrder(this.cart(),this.editingOrder()).pipe(finalize(()=>this.busy.set(false))).subscribe({next:()=>{this.cart.set([]);this.editingOrder.set(null);this.tab.set('orders');this.notice.set('Comanda a fost salvată.');this.load();},error:e=>this.fail(e)});}
  editOrder(o:Order){this.cart.set([...o.productsIds]);this.editingOrder.set(o.orderId);this.tab.set('catalog');this.notice.set('Alege produsele pentru comanda #'+o.orderId+'.');}
  cancelEdit(){this.editingOrder.set(null);this.cart.set([]);this.notice.set('');}
  deleteConfirmed(){const c=this.confirm();if(!c)return;this.busy.set(true);this.error.set('');const request=c.kind==='product'?this.api.deleteProduct(c.id):this.api.deleteOrder(c.id);request.pipe(finalize(()=>this.busy.set(false))).subscribe({next:()=>{this.confirm.set(null);this.notice.set(c.kind==='product'?'Produsul a fost șters.':'Comanda a fost ștearsă.');if(c.kind==='order'&&this.editingOrder()===c.id)this.cancelEdit();this.load();},error:e=>this.fail(e)});}
  names(o:Order){return o.productsIds.map(id=>this.products().find(p=>p.productId===id)?.name??'Produs #'+id).join(', ');}
  fail(e:Error){if(e instanceof HttpErrorResponse){if(e.status===401&&this.api.session())this.api.logout();const details=e.error?.detail??e.error?.message;this.error.set(e.status===0?'Backendul nu este disponibil. Verifică dacă este pornit.':details??(e.status===401?'Email sau parolă incorectă.':e.status===403?'Această operație este rezervată administratorului.':'Verifică datele introduse și încearcă din nou.'));}else this.error.set(e.message);}
}
