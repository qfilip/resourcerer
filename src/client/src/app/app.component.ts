import { Component, OnInit, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { DialogComponent } from "./components/common-ui/dialog/dialog.component";
import { ScratchpadComponent } from "./components/scratchpad/scratchpad.component";
import { SpinnerComponent } from "./components/common-ui/spinner/spinner.component";
import { PopupComponent } from "./components/common-ui/popup/popup.component";
import { UserController } from './controllers/user.controller';
import { UserService } from './services/user.service';
import { DialogWrapperComponent } from "./components/common-ui/dialog-wrapper/dialog-wrapper.component";
import { tap } from 'rxjs';
import { LogoutPage } from './pages/user/logout/logout.page';

@Component({
    selector: 'app-root',
    standalone: true,
    templateUrl: './app.component.html',
    styleUrl: './app.component.css',
    imports: [RouterOutlet, DialogComponent, SpinnerComponent, PopupComponent, LogoutPage, DialogWrapperComponent]
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
