import { Routes } from '@angular/router';
import { LoginComponent } from './pages/user/login/login.component';
import { HomeComponent } from './components/home/home.component';
import { CompanyOverviewPage } from './pages/company/overview/overview.page';
import { UomOverviewPage } from './pages/unitOfMeasure/uom-overview/uom-overview.page';
import { ItemsOverviewPage } from './pages/item/items-overview/items-overview.page';
import { UserManagementPage } from './pages/user/user-management/user-management.page';
import { AccountPage } from './pages/user/account/account.page';
import { BrowseItemsPage } from './pages/browse-item/browse-item/browse-items.page';

export const routes: Routes = [
    { path: '', component: HomeComponent },
    { path: 'login', component: LoginComponent },
    {
        path: 'home',
        component: HomeComponent,
        children: [
            { path: 'browse-items', component: BrowseItemsPage },
            { path: 'company', component: CompanyOverviewPage },
            // { path: 'companies', component: CompanyOverviewPage },
            { path: 'unitsofmeasure', component: UomOverviewPage },
            { path: 'items', component: ItemsOverviewPage },
            // { path: 'events', component: CompanyOverviewPage },
            { path: 'users', component: UserManagementPage },
            { path: 'account', component: AccountPage },
        ]
    },
];
