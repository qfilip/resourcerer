import { Component, computed, effect, inject } from '@angular/core';
import { DialogService } from '../../../services/dialog.service';
import { IDialogOptions } from '../../../models/components/IDialogOptions';

@Component({
  selector: 'app-dialog',
  standalone: true,
  imports: [],
  templateUrl: './dialog.component.html',
  styleUrl: './dialog.component.css'
})
export class DialogComponent {
  options: IDialogOptions | null = null;
  
  constructor(private service: DialogService) {
    effect(() => this.options = this.service.dialogOptions())
  }

  onButtonClicked(btnAction: () => void) {
    btnAction();
    this.service.close();
  }
}
