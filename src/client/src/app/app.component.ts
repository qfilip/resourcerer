import { Component, OnInit, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { DialogComponent } from "./components/dialog/dialog.component";
import { ScratchpadComponent } from "./components/scratchpad/scratchpad.component";
import { SpinnerComponent } from "./components/spinner/spinner.component";
import { PopupComponent } from "./components/popup/popup.component";
import { UserController } from './controllers/user.controller';
import { UserService } from './services/user.service';
import { LogoutComponent } from "./pages/user/components/logout/logout.component";
import { DialogWrapperComponent } from "./components/dialog-wrapper/dialog-wrapper.component";
import { tap } from 'rxjs';

@Component({
    selector: 'app-root',
    standalone: true,
    templateUrl: './app.component.html',
    styleUrl: './app.component.css',
    imports: [RouterOutlet, DialogComponent, ScratchpadComponent, SpinnerComponent, PopupComponent, LogoutComponent, DialogWrapperComponent]
})
export class AppComponent implements OnInit {
    router = inject(Router);
    userService = inject(UserService);
    userController = inject(UserController);
    
    ngOnInit() {
        this.userService.isLoggedIn()
            .subscribe({
                next: x => {
                    const route = x ? 'home' : 'login';
                    this.router.navigate([route]);
                }
            });
    }
}
