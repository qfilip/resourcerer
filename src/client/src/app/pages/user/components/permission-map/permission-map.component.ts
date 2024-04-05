import { Component, Input, computed, input, signal } from '@angular/core';
import { IAppUserDto } from '../../../../models/dtos/interfaces';
import { permissionsMap } from '../../../../models/dtos/constants';

@Component({
  selector: 'app-permission-map',
  standalone: true,
  imports: [],
  templateUrl: './permission-map.component.html',
  styleUrl: './permission-map.component.css'
})
export class PermissionMapComponent {
  @Input({
    required: true,
    alias: 'user',
    transform: (x: IAppUserDto) => PermissionMapComponent.mapPermissionTable(x)
  }) permissions: any;

  private static mapPermissionTable(user: IAppUserDto) {
    let map = [];
    for(const key in permissionsMap) {
      const userKey = user.permissionsMap[key];
      
      const hasPermissionSelector = userKey ?
        (x: string) => userKey.includes(x) :
        (_: string) => false;
      
      map.push({
        section: key,
        values: permissionsMap[key].map(x => {
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
