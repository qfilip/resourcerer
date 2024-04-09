import { Component, Input, inject } from '@angular/core';
import { IV1RegisterUser } from '../../../../models/dtos/interfaces';
import { PopupService } from '../../../../services/popup.service';
import { IPopup } from '../../../../models/components/IPopup';
import { PermissionMapComponent } from "../permission-map/permission-map.component";
import { UserPermission } from '../../../../models/components/UserPermission';
import { UserController } from '../../../../controllers/user.controller';
import { tryMapUser } from '../../../../functions/user.functions';

@Component({
    selector: 'register-user',
    standalone: true,
    templateUrl: './register-user.component.html',
    styleUrl: './register-user.component.css',
    imports: [PermissionMapComponent]
})
export class RegisterUserComponent {
    @Input({ required: true }) companyId!: string;
    
    private popupService = inject(PopupService);
    private userController = inject(UserController);
    
    userPermissions: UserPermission[] = [];

    registerUser(ev: Event, name: string, email: string, isAdmin: boolean) {
        ev.preventDefault();

        const result = tryMapUser(name, email, isAdmin, this.userPermissions);

        if(result.errors.length > 0) {
            const errors = result.errors.map(x => ({ message: x, type: 'warning' } as IPopup));
            this.popupService.many(errors);
            
            return;
        }

        const dto: IV1RegisterUser = {
            companyId: this.companyId,
            email: result.x.email,
            username: result.x.name,
            isAdmin: result.x.isAdmin,
            permissionsMap: result.x.permissionsMap
        }

        this.userController.registerUser(dto).subscribe({
            next: _ => this.popupService.success('User registered')
        });
    }
}
