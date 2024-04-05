import { Component, signal } from '@angular/core';
import { HomepageNav } from '../../models/components/HomepageNav';
import { AccountComponent } from "../../pages/user/account/account.page";

@Component({
    selector: 'app-home',
    standalone: true,
    templateUrl: './home.component.html',
    styleUrl: './home.component.css',
    imports: [AccountComponent]
})
export class HomeComponent {
  page$ = signal<HomepageNav>('Companies');
}
