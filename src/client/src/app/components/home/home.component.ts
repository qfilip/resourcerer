import { Component, signal } from '@angular/core';
import { HomepageNav } from '../../models/components/HomepageNav';
import { AccountPage } from "../../pages/user/account/account.page";
import { UserListComponent } from "../../pages/user/components/user-list/user-list.component";
import { CommonModule } from '@angular/common';
import { CompanyOverviewPage } from "../../pages/company/overview/overview.page";
import { UserManagementPage } from "../../pages/user/user-management/user-management.page";
import { UomOverviewPage } from "../../pages/unitOfMeasure/uom-overview/uom-overview.page";
import { ItemsOverviewPage } from '../../pages/item/items-overview/items-overview.page';

@Component({
    selector: 'app-home',
    standalone: true,
    templateUrl: './home.component.html',
    styleUrl: './home.component.css',
    imports: [CommonModule, ItemsOverviewPage, AccountPage, UserListComponent, CompanyOverviewPage, UserManagementPage, UomOverviewPage]
})
export class HomeComponent {
  page$ = signal<HomepageNav>('Company');

  iconList: { pageName: HomepageNav, icon: string }[] = [
    { pageName: 'Company', icon: 'ra ra-castle-emblem' },
    { pageName: 'Companies', icon: 'ra ra-hydra' },
    { pageName: 'Units of Measure', icon: 'ra ra-suits' },
    { pageName: 'Items', icon: 'ra ra-bone-knife' },
    { pageName: 'Events', icon: 'ra ra-lightning' },
    { pageName: 'Users', icon: 'ra ra-queen-crown' },
    { pageName: 'Account', icon: 'ra ra-aware' }
  ];
}
