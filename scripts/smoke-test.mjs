import assert from 'node:assert/strict';
const base=process.env.API_URL??'http://127.0.0.1:5156';
let checks=0,productId,orderId,admin,customer;
async function api(route,method='GET',body,token) {
  const response=await fetch(base+route,{method,headers:{'Content-Type':'application/json',...(token?{Authorization:'Bearer '+token}:{})},...(body!==undefined?{body:JSON.stringify(body)}:{}),signal:AbortSignal.timeout(10000)});
  const text=await response.text();let data;try{data=JSON.parse(text)}catch{data=text}
  return {status:response.status,data};
}
function equal(actual,expected,label){assert.deepEqual(actual,expected,label);checks++;console.log('PASS '+label);}
try {
  equal((await api('/api/health')).status,200,'health');
  equal((await api('/api/product/all')).status,401,'products require authentication');
  equal((await api('/api/login','POST',{email:'customer@example.com',password:'wrong'})).status,401,'wrong password');
  const a=await api('/api/login','POST',{email:'admin@example.com',password:'DemoAdmin123!'});equal(a.status,200,'admin login');admin=a.data.token;equal(a.data.roles.includes('Admin'),true,'admin role seeded');
  const c=await api('/api/login','POST',{email:'customer@example.com',password:'DemoCustomer123!'});equal(c.status,200,'customer login');customer=c.data.token;equal(c.data.roles.includes('Admin'),false,'customer is not admin');
  equal((await api('/api/register','POST',{email:'weak'+Date.now()+'@example.com',password:'weak'})).status,400,'weak password validation');
  const registration={email:'test'+Date.now()+'@example.com',password:'TestingUser123!'};
  equal((await api('/api/register','POST',registration)).status,201,'customer registration');
  equal((await api('/api/register','POST',registration)).status,409,'duplicate account conflict');
  const other=(await api('/api/login','POST',registration)).data.token;
  equal((await api('/api/product','POST',{name:'Forbidden',price:1},customer)).status,403,'customer cannot create products');
  equal((await api('/api/register-admin','POST',{email:'notadmin@example.com',password:'TestingAdmin123!'},customer)).status,403,'customer cannot create admins');
  equal((await api('/api/product','POST',{name:'Negative',price:-1},admin)).status,400,'negative product price validation');
  equal((await api('/api/product','POST',{name:'Precision',price:1.111},admin)).status,400,'price precision validation');
  equal((await api('/api/product/2147483647','GET',undefined,admin)).status,404,'missing product');
  const created=await api('/api/product','POST',{name:'Integration test',price:5},admin);equal(created.status,201,'product creation');productId=created.data.productId;
  const updated=await api('/api/product/'+productId,'PUT',{name:'Updated test',price:7.25},admin);equal(updated.data.price,7.25,'product update');
  const ordered=await api('/api/order','POST',{productsIds:[productId],totalAmount:.01},customer);equal(ordered.status,201,'order creation');orderId=ordered.data.orderId;equal(ordered.data.totalAmount,7.25,'server ignores forged total and calculates price');
  equal((await api('/api/order/'+orderId,'GET',undefined,customer)).status,200,'customer can read own order');
  equal((await api('/api/order/'+orderId,'GET',undefined,other)).status,404,'other customer cannot read order');
  equal((await api('/api/order/'+orderId,'PUT',{productsIds:[productId]},other)).status,404,'other customer cannot update order');
  equal((await api('/api/order/'+orderId,'DELETE',undefined,other)).status,404,'other customer cannot delete order');
  equal((await api('/api/order','POST',{productsIds:[productId,productId]},customer)).status,400,'duplicate products rejected');
  equal((await api('/api/order','POST',{productsIds:[2147483647]},customer)).status,400,'unknown products rejected');
  equal((await api('/api/order','POST',{productsIds:[]},customer)).status,400,'empty order rejected');
  equal((await api('/api/product/'+productId,'DELETE',undefined,admin)).status,400,'ordered product cannot be deleted');
  const list=(await api('/api/product/all','GET',undefined,customer)).data;const extra=list.find(p=>p.productId!==productId);
  const changed=await api('/api/order/'+orderId,'PUT',{productsIds:[productId,extra.productId],totalAmount:0},customer);equal(changed.data.totalAmount,7.25+extra.price,'order update recalculates total');
  equal((await api('/api/order/'+orderId,'DELETE',undefined,customer)).status,204,'order deletion');
  equal((await api('/api/order/'+orderId,'GET',undefined,customer)).status,404,'deleted order is missing');orderId=undefined;
  equal((await api('/api/product/'+productId,'DELETE',undefined,admin)).status,204,'product deletion');productId=undefined;
  equal((await api('/swagger/v1/swagger.json')).status,200,'Swagger schema');
  console.log('TOTAL '+checks+' checks passed');
}finally{
  if(orderId&&customer)await api('/api/order/'+orderId,'DELETE',undefined,customer);
  if(productId&&admin)await api('/api/product/'+productId,'DELETE',undefined,admin);
}
