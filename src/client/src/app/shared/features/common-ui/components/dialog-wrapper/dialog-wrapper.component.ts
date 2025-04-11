import { Component, ElementRef, signal, ViewChild } from '@angular/core';

@Component({
  standalone: true,
  selector: 'app-dialog-wrapper',
  imports: [],
  templateUrl: './dialog-wrapper.component.html',
  styleUrl: './dialog-wrapper.component.css'
})
export class DialogWrapperComponent {
  @ViewChild('dialog') private dialog!: ElementRef<HTMLDialogElement>;

  private _$visible = signal<boolean>(false);
  $visible = this._$visible.asReadonly();
  
  open() {
    this._$visible.set(true);
    this.showDialog(true);
  }

  close() {
    this._$visible.set(false);
    this.showDialog(false);
  }

  private showDialog(show: boolean) {
    const action = show
      ? () => this.dialog.nativeElement.showModal()
      : () => this.dialog.nativeElement.close();

    action();
  }
}
