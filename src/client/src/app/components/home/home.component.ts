import { Component, signal } from '@angular/core';
import { HomepageNav } from '../../models/components/HomepageNav';
import { AccountComponent } from "../../pages/user/account/account.page";
import { UserListComponent } from "../../pages/user/components/user-list/user-list.component";
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-home',
    standalone: true,
    templateUrl: './home.component.html',
    styleUrl: './home.component.css',
    imports: [CommonModule, AccountComponent, UserListComponent]
})
export class HomeComponent {
  page$ = signal<HomepageNav>('Companies');
}
