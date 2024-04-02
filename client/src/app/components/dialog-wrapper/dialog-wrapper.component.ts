import { Component } from '@angular/core';

@Component({
  selector: 'app-dialog-wrapper',
  standalone: true,
  imports: [],
  templateUrl: './dialog-wrapper.component.html',
  styleUrl: './dialog-wrapper.component.css'
})
export class DialogWrapperComponent {
  visible = false;
  
  open() {
    this.visible = true;
  }

  close() {
    this.visible = false;
  }
}
