import { Component, inject } from '@angular/core';
import { DialogService } from '../../services/dialog.service';
import { IDialogOptions } from '../../models/components/IDialogOptions';
import { SpinnerService } from '../../services/spinner.service';

@Component({
  selector: 'app-scratchpad',
  standalone: true,
  imports: [],
  templateUrl: './scratchpad.component.html',
  styleUrl: './scratchpad.component.css'
})
export class ScratchpadComponent {
  dialogService = inject(DialogService);
  spinnerService = inject(SpinnerService);

  openDialog() {
    this.dialogService.open({
      header: 'Hello',
      message: 'Testing dialog'
    } as IDialogOptions);
  }

  showSpinner() {
    this.spinnerService.show('working...');
    const x = setTimeout(() => {
      this.spinnerService.hide();
      clearTimeout(x);
    }, 1000)
  }

}
