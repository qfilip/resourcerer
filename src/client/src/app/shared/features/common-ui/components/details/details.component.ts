import { CommonModule } from '@angular/common';
import { Component, input, OnInit, output, signal } from '@angular/core';

@Component({
  standalone: true,
  selector: 'app-details',
  imports: [CommonModule],
  templateUrl: './details.component.html',
  styleUrl: './details.component.css'
})
export class DetailsComponent implements OnInit {
  $startOpen = input<boolean>(false);
  $selected = input<boolean>(false);
  
  private _$open = signal<boolean>(false);
  $open = this._$open.asReadonly();
  
  onToggle = output();

  ngOnInit(): void {
    this._$open.set(this.$startOpen());
  }

  toggle() {
    this._$open.update(x => !x);
    this.onToggle.emit();
  }
}
