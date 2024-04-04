import { Component, computed, effect, inject } from '@angular/core';
import { SpinnerService } from '../../services/spinner.service';

@Component({
  selector: 'app-spinner',
  standalone: true,
  imports: [],
  templateUrl: './spinner.component.html',
  styleUrl: './spinner.component.css'
})
export class SpinnerComponent {
  visible = false;
  message = 'Loading...';

  constructor(private service: SpinnerService) {
    effect(() => this.visible = this.service.isLoading());
    effect(() => this.message = this.service.message());
  }
}
