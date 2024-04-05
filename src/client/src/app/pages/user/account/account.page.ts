import { Component, computed, inject } from '@angular/core';
import { UserService } from '../../../services/user.service';
import { permissionsMap } from '../../../models/dtos/constants';
import { dictToArray } from '../../../functions/utility.functions';
import { IAppUserDto } from '../../../models/dtos/interfaces';
import { PermissionMapComponent } from "../components/permission-map/permission-map.component";

@Component({
    selector: 'account-page',
    standalone: true,
    templateUrl: './account.page.html',
    styleUrl: './account.page.css',
    imports: [PermissionMapComponent]
})
export class AccountComponent {
  private userService = inject(UserService);
  user$ = computed(() => this.userService.user());
}
