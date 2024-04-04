import { Component, OnInit, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { DialogComponent } from "./components/dialog/dialog.component";
import { ScratchpadComponent } from "./components/scratchpad/scratchpad.component";
import { SpinnerComponent } from "./components/spinner/spinner.component";
import { PopupComponent } from "./components/popup/popup.component";
import { UserController } from './controllers/user.controller';
import { UserService } from './services/user.service';

@Component({
    selector: 'app-root',
    standalone: true,
    templateUrl: './app.component.html',
    styleUrl: './app.component.css',
    imports: [RouterOutlet, DialogComponent, ScratchpadComponent, SpinnerComponent, PopupComponent]
})
export class AppComponent implements OnInit {
    router = inject(Router);
    userService = inject(UserService);
    userController = inject(UserController);
    
    ngOnInit() {
        const route = this.userService.isLoggedIn() ? 'home' : 'login';
        this.router.navigate([route]);
    }
}
