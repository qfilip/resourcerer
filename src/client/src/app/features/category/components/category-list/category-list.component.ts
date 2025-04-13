import { Component, effect, inject, signal } from '@angular/core';
import { ICategoryDto } from '../../../../shared/dtos/interfaces';
import { CategoryService } from '../../services/category.service';

@Component({
  selector: 'app-category-list',
  imports: [],
  templateUrl: './category-list.component.html',
  styleUrl: './category-list.component.css'
})
export class CategoryListComponent {
  private categoryService = inject(CategoryService);
  $categories = signal<ICategoryDto[]>([]);
  
  constructor() {
    effect(() => {
      const xs = this.categoryService.$categories();
      this.$categories.set(xs);
    });
  }
}
