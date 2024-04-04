import { Component, computed, inject } from '@angular/core';
import { UserService } from '../../../../services/user.service';
import { Observable } from 'rxjs';
import { IAppUserDto } from '../../../../models/dtos/interfaces';

@Component({
  selector: 'app-logout',
  standalone: true,
  imports: [],
  templateUrl: './logout.component.html',
  styleUrl: './logout.component.css'
})
export class LogoutComponent {
  private userService = inject(UserService);
  user$ = computed(() =>this.userService.user());
}
