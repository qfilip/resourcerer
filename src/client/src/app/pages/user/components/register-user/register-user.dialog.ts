import { Component, ViewChild } from '@angular/core';
import { DialogWrapperComponent } from "../../../../components/dialog-wrapper/dialog-wrapper.component";
import { IAppUserDto } from '../../../../models/dtos/interfaces';

@Component({
    selector: 'register-user-dialog',
    standalone: true,
    templateUrl: './register-user.dialog.html',
    styleUrl: './register-user.dialog.css',
    imports: [DialogWrapperComponent]
})
export class RegisterUserComponent {
    @ViewChild('wrapper') wrapper!: DialogWrapperComponent;
    private user = {} as IAppUserDto;
    
    open(companyId: string) {
        this.wrapper.open();
    }
}
