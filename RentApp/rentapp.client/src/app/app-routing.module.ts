import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './auth/auth.guard';

const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', loadComponent: () => import('./auth/login/login.component').then(m => m.LoginComponent) },
  { path: 'register', loadComponent: () => import('./auth/register/register.component').then(m => m.RegisterComponent) },
  { path: 'products', loadComponent: () => import('./products/product-list/product-list.component').then(m => m.ProductListComponent) },
  { path: 'products/:id', loadComponent: () => import('./products/product-details/product-details.component').then(m => m.ProductDetailsComponent) },
  { path: 'create', loadComponent: () => import('./products/product-form/product-form.component').then(m => m.ProductFormComponent), canActivate: [AuthGuard] },
  { path: 'account', loadComponent: () => import('./account/account.component').then(m => m.AccountComponent), canActivate: [AuthGuard] },
  { path: 'edit-account', loadComponent: () => import('./account/edit-account/edit-account.component').then(m => m.EditAccountComponent), canActivate: [AuthGuard] },
  { path: 'rentals/my', loadComponent: () => import('./rental/my-rentals/myrentals.component').then(m => m.MyRentalsComponent), canActivate: [AuthGuard] },
  { path: 'rent/:id', loadComponent: () => import('./rental/rent-product/rent-product.component').then(m => m.RentProductComponent), canActivate: [AuthGuard] },
  { path: 'favorites', loadComponent: () => import('./favorites/favorites.component').then(m => m.FavoritesComponent), canActivate: [AuthGuard] },
  { path: 'my-products', loadComponent: () => import('./products/my-products/my-products.component').then(m => m.MyProductsComponent), canActivate: [AuthGuard] },
  { path: 'edit-product/:id', loadComponent: () => import('./products/product-edit/product-edit.component').then(m => m.EditProductComponent), canActivate: [AuthGuard] }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
