import { Routes } from '@angular/router';
import { CompanyOverviewPage } from './pages/company/overview/company.overview.page';
import { UomOverviewPage } from './pages/unitOfMeasure/uom-overview/uom-overview.page';
import { ItemsOverviewPage } from './pages/item/items-overview/items-overview.page';
import { UserManagementPage } from './pages/user/user-management/user-management.page';
import { AccountPage } from './pages/user/account/account.page';
import { BrowseItemsPage } from './pages/item/browse-item/browse-items.page';
import { HomePage } from './pages/home/home.page';
import { LoginPage } from './pages/user/login/login.page';

export const routes: Routes = [
    { path: '', component: HomePage },
    { path: 'login', component: LoginPage },
    {
        path: 'home',
        component: HomePage,
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
