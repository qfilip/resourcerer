import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { IAppUserDto, IV1EditUser } from '../../../../models/dtos/interfaces';
import { PermissionMapComponent } from "../permission-map/permission-map.component";
import { UserPermission } from '../../../../models/components/UserPermission';
import { tryMapUser } from '../../../../functions/user.functions';
import { IPopup } from '../../../../models/components/IPopup';
import { PopupService } from '../../../../services/popup.service';
import { UserController } from '../../../../controllers/user.controller';

@Component({
    selector: 'user-edit',
    standalone: true,
    templateUrl: './user-edit.component.html',
    styleUrl: './user-edit.component.css',
    imports: [PermissionMapComponent]
})
export class UserEditComponent {
  @Input({ required: true }) user!: IAppUserDto;
  @Output() onUserEdited = new EventEmitter<IAppUserDto>();
  
  private popupService = inject(PopupService);
  private userController = inject(UserController);

  permissions: UserPermission[] = [];

  editUser(ev: Event, name: string, email: string, isAdmin: boolean) {
    ev.preventDefault();

    const result = tryMapUser(name, email, isAdmin, this.permissions);
    if(result.errors.length > 0) {
        const errors = result.errors.map(x => ({ message: x, type: 'warning' } as IPopup));
        this.popupService.many(errors);
        
        return;
    }

    const dto: IV1EditUser = {
      userId: this.user.id,
      email: result.x.email,
      isAdmin: result.x.isAdmin,
      permissionsMap: result.x.permissionsMap
    };

    this.userController.editUser(dto)
      .subscribe({
        next: x => this.onUserEdited.emit(x)
      });
  }
}
