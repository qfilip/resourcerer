import { Component, signal } from '@angular/core';
import { HomepageNav } from '../../models/components/HomepageNav';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterOutlet } from '@angular/router';

@Component({
    selector: 'app-home',
    standalone: true,
    templateUrl: './home.component.html',
    styleUrl: './home.component.css',
    imports: [CommonModule, RouterLink, RouterOutlet]
})
export class HomeComponent {
  page$ = signal<HomepageNav>('Company');

  pages: { pageName: HomepageNav, route: string, icon: string }[] = [
    { pageName: 'Company', route: 'company', icon: 'ra ra-castle-emblem' },
    // { pageName: 'Companies', route: 'companies', icon: 'ra ra-hydra' },
    { pageName: 'Units of Measure', route: 'unitsofmeasure', icon: 'ra ra-suits' },
    { pageName: 'Items', route: 'items', icon: 'ra ra-bone-knife' },
    // { pageName: 'Events', route: 'events', icon: 'ra ra-lightning' },
    { pageName: 'Users', route: 'users', icon: 'ra ra-queen-crown' },
    { pageName: 'Account', route: 'account', icon: 'ra ra-aware' }
  ];
}
