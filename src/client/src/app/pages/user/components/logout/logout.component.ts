import { Component, computed, inject } from '@angular/core';
import { UserService } from '../../../../services/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-logout',
  standalone: true,
  imports: [],
  templateUrl: './logout.component.html',
  styleUrl: './logout.component.css'
})
export class LogoutComponent {
  private router = inject(Router);
  private userService = inject(UserService);
  
  user$ = computed(() => this.userService.user());

  logout() {
    this.userService.clearUser();
    this.router.navigate(['login']);
  }
}
