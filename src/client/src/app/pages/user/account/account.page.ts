import { Component, computed, inject } from '@angular/core';
import { UserService } from '../../../services/user.service';
import { PermissionMapComponent } from '../../../components/user/permission-map/permission-map.component';

@Component({
    selector: 'account-page',
    standalone: true,
    templateUrl: './account.page.html',
    styleUrl: './account.page.css',
    imports: [PermissionMapComponent]
})
export class AccountPage {
  private userService = inject(UserService);
  user$ = computed(() => this.userService.user());
}
