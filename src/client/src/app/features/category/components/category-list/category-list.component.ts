import { Component, effect, inject, output, signal } from '@angular/core';
import { ICategoryDto } from '../../../../shared/dtos/interfaces';
import { CategoryService } from '../../services/category.service';
import { CategoryComponent } from '../category/category.component';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-category-list',
  templateUrl: './category-list.component.html',
  styleUrl: './category-list.component.css',
  imports: [CommonModule, CategoryComponent]
})
export class CategoryListComponent {
  private categoryService = inject(CategoryService);
  $categories = signal<ICategoryDto[]>([]);
  
  onCreate = output();
  onUpdate = output<ICategoryDto>();
  onRemove = output<ICategoryDto>();

  constructor() {
    effect(() => {
      const xs = this.categoryService.$categoryTree();
      this.$categories.set(xs);
    });
  }
}
