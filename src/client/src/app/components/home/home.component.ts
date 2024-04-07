import { Component, signal } from '@angular/core';
import { HomepageNav } from '../../models/components/HomepageNav';
import { AccountPage } from "../../pages/user/account/account.page";
import { UserListComponent } from "../../pages/user/components/user-list/user-list.component";
import { CommonModule } from '@angular/common';
import { CompanyOverviewPage } from "../../pages/company/overview/overview.page";

@Component({
    selector: 'app-home',
    standalone: true,
    templateUrl: './home.component.html',
    styleUrl: './home.component.css',
    imports: [CommonModule, AccountPage, UserListComponent, CompanyOverviewPage]
})
export class HomeComponent {
  page$ = signal<HomepageNav>('Company');
}
