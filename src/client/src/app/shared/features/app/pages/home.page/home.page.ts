import { Component, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { PopupComponent } from "../../../common-ui/components/popup/popup.component";
import { LoaderComponent } from "../../../common-ui/components/loader/loader.component";
import { SimpleDialogComponent } from "../../../common-ui/components/simple-dialog/simple-dialog.component";
import { AppHeaderComponent } from "../../components/app-header/app-header.component";

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [PopupComponent, RouterOutlet, LoaderComponent, SimpleDialogComponent, AppHeaderComponent],
  templateUrl: './home.page.html',
  styleUrl: './home.page.css'
})
export class HomePageComponent {
  router = inject(Router);
}
