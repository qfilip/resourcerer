import { Component, Input, inject } from '@angular/core';
import { IV1RegisterUser } from '../../../../models/dtos/interfaces';
import { PopupService } from '../../../../services/popup.service';
import { IPopup } from '../../../../models/components/IPopup';
import { PermissionMapComponent } from "../permission-map/permission-map.component";
import { UserPermission } from '../../../../models/components/UserPermission';
import { UserController } from '../../../../controllers/user.controller';

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
    
    private permissionMap: { [key: string]: string[] } = {}; 

    mapPermissions(xs: UserPermission[]) {
        xs.forEach(x => {
            const ps = x.permissions
                .filter(p => p.hasPermission)
                .map(p => p.name);

            this.permissionMap[x.section] = ps;
        });
    }

    registerUser(name: string, email: string, isAdmin: boolean) {
        const validate = (x: string, error: string) =>
            !x || x.length === 0 ? error : null;

        const errors = [
            validate(name, 'Name cannot be empty'),
            validate(email, 'Email cannot be empty'),
        ]
        .filter(x => x !== null)
        .map(x => ({ message: x, type: 'warning' } as IPopup));

        if(errors.length > 0) {
            this.popupService.many(errors);
            return;
        }

        const dto: IV1RegisterUser = {
            companyId: this.companyId,
            email: email,
            username: name,
            isAdmin: isAdmin,
            permissionsMap: this.permissionMap
        }

        this.userController.registerUser(dto).subscribe({
            next: x => this.popupService.success('User registered')
        });
    }
}
