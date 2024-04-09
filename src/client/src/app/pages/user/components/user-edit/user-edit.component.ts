import { Component, Input, inject } from '@angular/core';
import { IAppUserDto } from '../../../../models/dtos/interfaces';
import { PermissionMapComponent } from "../permission-map/permission-map.component";
import { UserPermission } from '../../../../models/components/UserPermission';
import { tryMapUser } from '../../../../functions/user.functions';
import { IPopup } from '../../../../models/components/IPopup';
import { PopupService } from '../../../../services/popup.service';

@Component({
    selector: 'user-edit',
    standalone: true,
    templateUrl: './user-edit.component.html',
    styleUrl: './user-edit.component.css',
    imports: [PermissionMapComponent]
})
export class UserEditComponent {
  @Input({ required: true }) user: IAppUserDto | null = null;
  
  private popupService = inject(PopupService);

  permissions: UserPermission[] = [];

  editUser(ev: Event, name: string, email: string, isAdmin: boolean) {
    ev.preventDefault();

    const result = tryMapUser(name, email, isAdmin, this.permissions);
    if(result.errors.length > 0) {
        const errors = result.errors.map(x => ({ message: x, type: 'warning' } as IPopup));
        this.popupService.many(errors);
        
        return;
    }

    // call edit user controller function
  }
}
