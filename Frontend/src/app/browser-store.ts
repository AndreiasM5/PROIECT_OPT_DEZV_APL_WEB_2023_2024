import type { Product, Order, Draft } from './models';
export const KEY = 'unibuc.dotnet.store.v1';
interface State { version: number; products: Product[]; orders: Order[]; nextProductId: number; nextOrderId: number; }
export class BrowserStore {
  private storage: Pick<Storage, 'getItem' | 'setItem'>;
  constructor(storage: Pick<Storage, 'getItem' | 'setItem'>) { this.storage = storage; }
  private read(): State {
    let raw;
    try { raw = this.storage.getItem(KEY); } catch { throw new Error('Browserul nu permite accesul la datele locale.'); }
    if(raw === null) {
      const state: State = {version:1, products:[{productId:1,name:'Water',price:2},{productId:2,name:'Jack Daniels',price:10},{productId:3,name:'Pepsi',price:4}],orders:[],nextProductId:4,nextOrderId:1};
      this.write(state); return state;
    }
    try {
      const s = JSON.parse(raw) as State;
      if(s.version !== 1 || !Array.isArray(s.products) || !Array.isArray(s.orders) || !Number.isSafeInteger(s.nextProductId) || !Number.isSafeInteger(s.nextOrderId))throw new Error();
      const ids = new Set<number>();
      for(const p of s.products) { this.valid(p); if(!Number.isSafeInteger(p.productId) || p.productId < 1 || ids.has(p.productId) || p.productId >= s.nextProductId)throw new Error(); ids.add(p.productId); }
      const orders = new Set<number>();
      for(const o of s.orders) {
        if(!Number.isSafeInteger(o.orderId) || o.orderId < 1 || o.orderId >= s.nextOrderId || orders.has(o.orderId) || !Number.isFinite(o.totalAmount) || o.totalAmount < 0 || !Array.isArray(o.productsIds) || o.productsIds.length === 0 || o.productsIds.some(id=>!ids.has(id)))throw new Error();
        orders.add(o.orderId);
      }
      return s;
    } catch { throw new Error('Datele locale nu pot fi citite. Încearcă un alt profil sau șterge datele acestui site.'); }
  }
  private write(s: State) { try { this.storage.setItem(KEY, JSON.stringify(s)); } catch { throw new Error('Browserul nu permite salvarea. Verifică setările de stocare locală.'); } }
  private valid(d: Draft): Draft {
    if(typeof d.name !== 'string' || !d.name.trim() || d.name.length > 200 || !Number.isFinite(d.price) || d.price < 0 || d.price > 9999999999.99 || Math.abs(d.price * 100 - Math.round(d.price * 100)) > .0001)throw new Error('Completează un nume și un preț valid, cu cel mult două zecimale.');
    return {name:d.name.trim(),price:Math.round(d.price*100)/100};
  }
  products() { return this.read().products; }
  orders() { return this.read().orders.sort((a,b)=>b.orderId-a.orderId); }
  saveProduct(d: Draft, id: number | null): Product {
    const value=this.valid(d),s=this.read();
    let p: Product;
    if(id === null) { p={...value,productId:s.nextProductId++}; s.products.push(p); }
    else { p=s.products.find(p=>p.productId===id)!; if(!p)throw new Error('Produsul nu există.'); Object.assign(p,value); }
    this.write(s); return p;
  }
  deleteProduct(id: number) {
    const s=this.read();
    if(!s.products.some(p=>p.productId===id))throw new Error('Produsul nu există.');
    if(s.orders.some(o=>o.productsIds.includes(id)))throw new Error('Produsul este folosit într-o comandă și nu poate fi șters.');
    s.products=s.products.filter(p=>p.productId!==id); this.write(s);
  }
  saveOrder(ids: number[], id: number | null): Order {
    const s=this.read();
    if(ids.length===0 || ids.length>100 || new Set(ids).size!==ids.length || ids.some(id=>!s.products.some(p=>p.productId===id)))throw new Error('Alege între 1 și 100 de produse distincte, existente.');
    const total=Math.round(s.products.filter(p=>ids.includes(p.productId)).reduce((sum,p)=>sum+p.price,0)*100)/100;
    let o: Order;
    if(id === null) { o={orderId:s.nextOrderId++,productsIds:[...ids],totalAmount:total}; s.orders.push(o); }
    else { o=s.orders.find(o=>o.orderId===id)!; if(!o)throw new Error('Comanda nu există.'); o.productsIds=[...ids];o.totalAmount=total; }
    this.write(s); return o;
  }
  deleteOrder(id: number) { const s=this.read(); if(!s.orders.some(o=>o.orderId===id))throw new Error('Comanda nu există.'); s.orders=s.orders.filter(o=>o.orderId!==id);this.write(s); }
}
