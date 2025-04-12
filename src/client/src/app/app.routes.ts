import { Routes } from '@angular/router';
import { HomePageComponent } from './shared/features/app/pages/home.page/home.page';
import { LoginPage } from './features/user/pages/login/login.page';

export const routes: Routes = [
    { path: '', component: HomePageComponent },
    { path: 'home', component: HomePageComponent },
    { path: 'login', component: LoginPage }
];
