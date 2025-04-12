import { Component, effect, inject, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { PopupComponent } from "./shared/features/common-ui/components/popup/popup.component";
import { LoaderComponent } from "./shared/features/common-ui/components/loader/loader.component";
import { SimpleDialogComponent } from "./shared/features/common-ui/components/simple-dialog/simple-dialog.component";
import { AppHeaderComponent } from "./shared/features/app/components/app-header/app-header.component";
import { UserService } from './features/user/services/user.service';

@Component({
	selector: 'app-root',
	imports: [PopupComponent, LoaderComponent, SimpleDialogComponent, AppHeaderComponent, RouterOutlet],
	standalone: true,
	templateUrl: './app.component.html',
	styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
	router = inject(Router);
    userService = inject(UserService);
    
    constructor() {
        effect(() => {
            const user = this.userService.user$();
			const route = user !== null ? 'home' : 'login';
			this.router.navigate([route]);
        });
    }

	ngOnInit(): void {
		this.userService.getFromCache();
	}
}
