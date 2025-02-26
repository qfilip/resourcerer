import { Component, OnInit, inject } from '@angular/core';
import { PopupService } from '../../../services/popup.service';
import { PopupSnake } from '../../../models/components/IPopup';
import { CommonModule } from '@angular/common';
import { Observable, tap } from 'rxjs';

@Component({
  selector: 'app-popup',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './popup.component.html',
  styleUrl: './popup.component.css'
})
export class PopupComponent implements OnInit {
  private service = inject(PopupService);
  popupSnake$: Observable<PopupSnake> = new Observable<PopupSnake>(obs => obs.next({ head: null, tail: []}));

  ngOnInit(): void {
    this.popupSnake$ = this.service.popupSnake
      .pipe(tap(x => {
        if(x.head) {
          this.visible();
        }
      }));
  }

  toggleExpanded() {
    switch (this.state) {
      case 'expanded':
        this.state = 'visible';
        break;
      case 'visible':
        this.state = 'expanded';
        break;
    }
  }

  hide() {
    this.state = 'hidden';
  }

  clear() {
    this.state = 'hidden';
    // this.popups.x = null;
    // this.popups.xs = [];
  }

  private visible() {
    if(this.state === 'hidden') {
      this.state = 'visible';
    }
  }

  state: 'expanded' | 'visible' | 'hidden' = 'hidden';
}
