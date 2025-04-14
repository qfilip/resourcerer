import { CommonModule } from '@angular/common';
import { Component, input } from '@angular/core';

@Component({
  standalone: true,
  selector: 'app-form-error',
  imports: [CommonModule],
  templateUrl: './form-error.component.html',
  styleUrl: './form-error.component.css'
})
export class FormErrorComponent {
  $errors = input.required<string[]>();
  $expandable = input<boolean>(true);
}
