import { Component, computed, inject } from '@angular/core';
import { UserService } from '../../../services/user.service';
import { permissionsMap } from '../../../models/dtos/constants';
import { dictToArray } from '../../../functions/utility.functions';
import { IAppUserDto } from '../../../models/dtos/interfaces';

@Component({
  selector: 'account-page',
  standalone: true,
  imports: [],
  templateUrl: './account.page.html',
  styleUrl: './account.page.css'
})
export class AccountComponent {
  private userService = inject(UserService);
  user$ = computed(() => this.userService.user());
  
  userPermissions = AccountComponent.mapPermissionTable(permissionsMap, this.user$()!);

  private static mapPermissionTable(allPermissions: { [key: string]: string[] }, user: IAppUserDto) {
    let map = [];
    for(const key in allPermissions) {
      const userKey = user.permissionsMap[key];
      
      const hasPermissionSelector = userKey ?
        (x: string) => userKey.includes(x) :
        (_: string) => false;
      
      map.push({
        section: key,
        values: allPermissions[key].map(x => {
          return {
            permission: x,
            hasPermission: hasPermissionSelector(x)
          }
        })
      })
    }

    return map;
  }
}
