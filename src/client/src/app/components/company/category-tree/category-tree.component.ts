import { Component, Input } from '@angular/core';
import { ICategoryDto } from '../../../models/dtos/interfaces';

@Component({
  selector: 'category-tree',
  standalone: true,
  imports: [],
  templateUrl: './category-tree.component.html',
  styleUrl: './category-tree.component.css'
})
export class CategoryTreeComponent {
  @Input({ required: true }) category: ICategoryDto | null = null;
}
