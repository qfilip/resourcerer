import { Routes } from '@angular/router';
import { LoginComponent } from './pages/user/login/login.component';
import { HomeComponent } from './components/home/home.component';

export const routes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'home', component: HomeComponent },
    { path: 'login', component: LoginComponent }
];
