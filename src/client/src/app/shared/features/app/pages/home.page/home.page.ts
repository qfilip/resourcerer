import { Component, effect, inject, signal } from '@angular/core';
import { UserService } from '../../../../../features/user/services/user.service';
import { IAppUserDto } from '../../../../dtos/interfaces';
import { Router, RouterLink, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterOutlet, RouterLink],
  templateUrl: './home.page.html',
  styleUrl: './home.page.css'
})
export class HomePageComponent {
  private router = inject(Router);
  private userService = inject(UserService);
  $user = signal<IAppUserDto | null>(null);

  constructor() {
    effect(() => {
      const user = this.userService.$user();
      if(!user) return;
      this.$user.set(user);
    });
  }
}
