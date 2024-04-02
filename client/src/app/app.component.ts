import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { DialogComponent } from "./components/dialog/dialog.component";
import { ScratchpadComponent } from "./components/scratchpad/scratchpad.component";
import { SpinnerComponent } from "./components/spinner/spinner.component";

@Component({
    selector: 'app-root',
    standalone: true,
    templateUrl: './app.component.html',
    styleUrl: './app.component.css',
    imports: [RouterOutlet, DialogComponent, ScratchpadComponent, SpinnerComponent]
})
export class AppComponent {

}
