import { Component, effect, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { UserService } from '../../../../../features/user/services/user.service';
import { IAppUserDto } from '../../../../dtos/interfaces';
import { NavigationEnd, Router, RouterLink, RouterOutlet } from '@angular/router';
import { filter, map, Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink],
  templateUrl: './home.page.html',
  styleUrl: './home.page.css'
})
export class HomePageComponent implements OnInit, OnDestroy {
  private router = inject(Router);
  private userService = inject(UserService);
  private sub: Subscription | null = null;

  navLinks: Link[] = [
    { title: 'Categories', route: 'categories', icon: 'las la-folder-minus' },
    { title: 'Items', route: 'items', icon: 'las la-vial' },
    { title: 'Units of measure', route: 'uoms', icon: 'las la-ruler'}
  ]
  
  pages: Link[] = this.navLinks.concat([
    { title: 'Instances', route: 'instances', icon: ''}
  ]);

  $user = signal<IAppUserDto | null>(null);
  $page = signal<Link>(this.pages[0])

  constructor() {
    effect(() => {
      const user = this.userService.$user();
      if (!user) return;
      this.$user.set(user);
    });
  }

  ngOnInit() {
    this.router.navigate(['/home/categories']);

    this.sub = this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      map((event) => event as NavigationEnd)
    ).subscribe({
      next: x => {
        const i = x.url.split('/');
        const last = i[i.length - 1].split('?')[0];
        const page = this.pages.find(x => x.route === last);
        if (page)
          this.$page.set(page);
      }
    });
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }
}

type Link = { 
  title: string,
  route: string,
  icon: string
};
