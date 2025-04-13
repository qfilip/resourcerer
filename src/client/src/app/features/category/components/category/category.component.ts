import { Component, input, signal } from '@angular/core';
import { ICategoryDto } from '../../../../shared/dtos/interfaces';

@Component({
  standalone: true,
  selector: 'app-category',
  imports: [],
  templateUrl: './category.component.html',
  styleUrl: './category.component.css'
})
export class CategoryComponent {
  $category = input.required<ICategoryDto>();
}
