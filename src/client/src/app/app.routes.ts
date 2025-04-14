import { Routes } from '@angular/router';
import { HomePageComponent } from './shared/features/app/pages/home.page/home.page';
import { LoginPage } from './features/user/pages/login/login.page';
import { CategoryPage } from './features/category/pages/category/category.page';
import { ItemPage } from './features/item/pages/item/item.page';
import { UomsPage } from './features/uoms/pages/uoms/uoms.page';

export const routes: Routes = [
    { path: '', component: HomePageComponent },
    {
        path: 'home',
        component: HomePageComponent,
        children: [
            { path: 'categories', component: CategoryPage },
            { path: 'items', component: ItemPage },
            { path: 'uoms', component: UomsPage },
        ]
    },
    { path: 'login', component: LoginPage }
];
