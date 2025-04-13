import { Component, effect, inject, OnInit, signal } from '@angular/core';
import { UserService } from '../../../../../features/user/services/user.service';
import { Observable } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
	selector: 'app-header',
	imports: [CommonModule],
	templateUrl: './app-header.component.html',
	styleUrl: './app-header.component.css'
})
export class AppHeaderComponent {
	private userService = inject(UserService);
	private _$loggedIn = signal<boolean>(false); 
	$loggedIn = this._$loggedIn.asReadonly();
	
	constructor() {
		effect(() => {
			const user = this.userService.$user();
			const loggedIn = user !== null;
			this._$loggedIn.set(loggedIn);
		});
	}

	logout() {
		this.userService.logout();
	}
}
