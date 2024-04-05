import { Component, inject } from '@angular/core';
import { DialogService } from '../../services/dialog.service';
import { IDialogOptions } from '../../models/components/IDialogOptions';
import { SpinnerService } from '../../services/spinner.service';
import { PopupService } from '../../services/popup.service';
import { PopupType } from '../../models/components/IPopup';

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
  popupService = inject(PopupService);

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
    }, 5000)
  }

  sendPopup(t: PopupType) {
    if(t === 'info') this.popupService.info('info');
    if(t === 'success') this.popupService.success('success');
    if(t === 'warning') this.popupService.warning('warning');
    if(t === 'error') this.popupService.error('error');
  }
}
