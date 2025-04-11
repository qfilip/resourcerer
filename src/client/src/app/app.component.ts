import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { LoaderComponent } from './features/common-ui/components/loader/loader.component';
import { PopupComponent } from './features/common-ui/components/popup/popup.component';
import { SimpleDialogComponent } from './features/common-ui/components/simple-dialog/simple-dialog.component';

@Component({
	selector: 'app-root',
	imports: [CommonModule, LoaderComponent, SimpleDialogComponent, RouterOutlet, PopupComponent],
	standalone: true,
	templateUrl: './app.component.html',
	styleUrl: './app.component.css'
})
export class AppComponent {
	router = inject(Router);
}
