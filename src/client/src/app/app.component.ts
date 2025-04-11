import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { HomePageComponent } from "./shared/features/app/pages/home.page/home.page";

@Component({
	selector: 'app-root',
	imports: [HomePageComponent],
	standalone: true,
	templateUrl: './app.component.html',
	styleUrl: './app.component.css'
})
export class AppComponent {
}
