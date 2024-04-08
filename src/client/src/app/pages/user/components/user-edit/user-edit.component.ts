import { Component, Input } from '@angular/core';
import { IAppUserDto } from '../../../../models/dtos/interfaces';
import { PermissionMapComponent } from "../permission-map/permission-map.component";

@Component({
    selector: 'user-edit',
    standalone: true,
    templateUrl: './user-edit.component.html',
    styleUrl: './user-edit.component.css',
    imports: [PermissionMapComponent]
})
export class UserEditComponent {
  @Input({ required: true }) user: IAppUserDto | null = null;
}
