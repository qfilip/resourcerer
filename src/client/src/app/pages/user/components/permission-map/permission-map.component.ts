import { Component, EventEmitter, Input, Output, computed, input, signal } from '@angular/core';
import { IAppUserDto } from '../../../../models/dtos/interfaces';
import { UserPermission } from '../../../../models/components/UserPermission';
import { permissionsMap } from '../../../../models/dtos/constants';

@Component({
  selector: 'permission-map',
  standalone: true,
  imports: [],
  templateUrl: './permission-map.component.html',
  styleUrl: './permission-map.component.css'
})
export class PermissionMapComponent {
  @Input() editable = false;
  @Output() onMapChanged = new EventEmitter<UserPermission[]>();
  
  @Input({
    required: true,
    alias: 'user',
    transform: (x: IAppUserDto) => PermissionMapComponent.mapPermissionTable(x)
   }) permissions: UserPermission[] = [];
  
  emitMap() {
    this.onMapChanged.emit(this.permissions);
  }

  private static mapPermissionTable(user: IAppUserDto) {
    let map: UserPermission[] = [];
    for(const key in permissionsMap) {
      const userKey = user.permissionsMap[key];
      
      const hasPermissionSelector = userKey ?
        (x: string) => userKey.includes(x) :
        (_: string) => false;
      
      map.push({
        section: key,
        permissions: permissionsMap[key].map(x => {
          return {
            name: x,
            hasPermission: hasPermissionSelector(x)
          }
        })
      })
    }

    return map;
  }
}
