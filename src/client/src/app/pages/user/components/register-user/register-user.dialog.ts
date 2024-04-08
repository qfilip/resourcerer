import { Component, ViewChild } from '@angular/core';
import { DialogWrapperComponent } from "../../../../components/dialog-wrapper/dialog-wrapper.component";

@Component({
    selector: 'register-user-dialog',
    standalone: true,
    templateUrl: './register-user.dialog.html',
    styleUrl: './register-user.dialog.css',
    imports: [DialogWrapperComponent]
})
export class RegisterUserComponent {
    @ViewChild('wrapper') wrapper!: DialogWrapperComponent;

    open() {
        this.wrapper.open();
    }
}
