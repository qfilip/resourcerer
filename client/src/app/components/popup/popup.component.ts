import { Component, effect } from '@angular/core';
import { PopupService } from '../../services/popup.service';
import { IPopup } from '../../models/components/IPopup';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-popup',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './popup.component.html',
  styleUrl: './popup.component.css'
})
export class PopupComponent {
  popups: { x: IPopup | null, xs: IPopup[] } = { x: null, xs: [] }

  constructor(private service: PopupService) {
    effect(() => {
      const newPopup = this.service.popup();
      if(!newPopup) {
        return;
      }

      if(!this.popups.x) {
        this.popups.x = newPopup;
        this.setVisibility();

        return;
      }

      this.popups.xs.unshift(this.popups.x);
      this.popups.x = newPopup;
      this.setVisibility();
    })
  }

  expand() {
    this.state = 'expanded';
  }

  collapse() {
    this.state = 'visible';
  }

  private setVisibility() {
    if(this.state === 'hidden') {
      this.state = 'visible';
    }
  }


  state: 'expanded' | 'visible' | 'hidden' = 'hidden';
}
