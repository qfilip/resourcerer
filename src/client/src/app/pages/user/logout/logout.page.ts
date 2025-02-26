import { Component, computed, inject } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../../../services/user.service';

@Component({
  selector: 'app-logout',
  standalone: true,
  imports: [],
  templateUrl: './logout.page.html',
  styleUrl: './logout.page.css'
})
export class LogoutPage {
  private router = inject(Router);
  private userService = inject(UserService);
  
  user$ = computed(() => this.userService.user());

  logout() {
    this.userService.clearUser();
    this.router.navigate(['login']);
  }
}
