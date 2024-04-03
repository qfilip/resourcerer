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
        this.setVisibility(newPopup.duration);

        return;
      }

      this.popups.xs.unshift(this.popups.x);
      this.popups.x = newPopup;
      this.setVisibility(newPopup.duration);
    })
  }

  expand() {
    this.state = 'expanded';
  }

  collapse() {
    if(this.state === 'expanded') {
      this.state = 'visible';
      this.hideAfter(3000);
    }
  }

  hide() {
    if(this.state !== 'expanded') {
      console.log('hiding')
      this.state = 'hidden';
    }
  }

  private setVisibility(hideAfter: number) {
    if(this.state === 'hidden') {
      this.state = 'visible';
    }

    this.hideAfter(hideAfter);
  }

  hideAfter(time: number) {
    const t = setTimeout(() => {
      this.hide();
      clearTimeout(t);
    }, time);
  }

  state: 'expanded' | 'visible' | 'hidden' = 'hidden';
}
