import { Component, computed, effect, inject, output, signal } from '@angular/core';
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
  
  $categoryTrees = signal<ICategoryDto[]>([]);
  $selectedTree = computed(() => this.categoryService.$selectedCategory());
  private $query = signal<string>('');

  onSelected = output<ICategoryDto>();

  $displayed = computed(() => {
    const items = this.$categoryTrees();
    const query = this.$query().toLowerCase();
    
    return items.filter(x => 
      x.name.toLowerCase().includes(query) ||
      x.id.toLowerCase().includes(query));
  });

  constructor() {
    effect(() => {
      const xs = this.categoryService.$categoryTrees();
      this.$categoryTrees.set(xs);
    });
  }

  onQueryChanged = (query: string) => this.$query.set(query);
}
