import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { HomepageNav } from '../../models/components/HomepageNav';
import { CommonModule } from '@angular/common';
import { NavigationEnd, Router, RouterLink, RouterOutlet } from '@angular/router';
import { Subject, filter, map, takeUntil } from 'rxjs';

@Component({
  selector: 'app-home',
  standalone: true,
  templateUrl: './home.page.html',
  styleUrl: './home.page.css',
  imports: [CommonModule, RouterLink, RouterOutlet]
})
export class HomePage implements OnInit, OnDestroy {

  private router = inject(Router);
  private subbed = new Subject();

  ngOnInit(): void {
    this.router.events.pipe(
      takeUntil(this.subbed),
      filter(event => event instanceof NavigationEnd),
      map((event) => event as NavigationEnd)
    ).subscribe({
        next: (e) => {
          const i = e.url.split('/');
          const last = i[i.length - 1];
          const route = this.pages.filter(x => x.route === last);
          
          if(route.length === 1) {
            this.page$.set(route[0].pageName);
          }
          else {
            this.page$.set('Browse items');
          }
        }
      });
  }

  ngOnDestroy(): void {
    this.subbed.next(0);
    this.subbed.complete();
  }


  page$ = signal<HomepageNav>('Company');

  pages: { pageName: HomepageNav, route: string, icon: string }[] = [
    { pageName: 'Browse items', route: 'browse-items', icon: 'ra ra-telescope'},
    { pageName: 'Company', route: 'company', icon: 'ra ra-castle-emblem' },
    { pageName: 'Units of Measure', route: 'unitsofmeasure', icon: 'ra ra-suits' },
    { pageName: 'Items', route: 'items', icon: 'ra ra-bone-knife' },
    // { pageName: 'Events', route: 'events', icon: 'ra ra-lightning' },
    { pageName: 'Users', route: 'users', icon: 'ra ra-queen-crown' },
    { pageName: 'Account', route: 'account', icon: 'ra ra-aware' }
  ];
}
