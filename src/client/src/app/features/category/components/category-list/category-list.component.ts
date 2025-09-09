import { Component, computed, effect, inject, output, signal } from '@angular/core';
import { ICategoryDto } from '../../../../shared/dtos/interfaces';
import { CategoryService } from '../../services/category.service';
import { CategoryComponent } from '../category/category.component';
import { CommonModule } from '@angular/common';
import { SearchListComponent } from "../../../../shared/features/common-ui/components/search-list/search-list.component";
import { SearchListRowTemplateDirective } from '../../../../shared/features/common-ui/directives/search-list-row-template.directive';

@Component({
  standalone: true,
  selector: 'app-category-list',
  templateUrl: './category-list.component.html',
  styleUrl: './category-list.component.css',
  imports: [CommonModule, CategoryComponent, SearchListComponent, SearchListRowTemplateDirective]
})
export class CategoryListComponent {
  private categoryService = inject(CategoryService);
  
  $categoryTrees = this.categoryService.$categoryTrees;
  $selectedTree = computed(() => this.categoryService.$selectedCategory());

  onSelected = output<ICategoryDto>();
  displayFilter = (query: string) => (x: ICategoryDto) => {
    return x.name.toLowerCase().includes(query) ||
      x.id.toLowerCase().includes(query);
  }

  typeToken = {} as ICategoryDto;
}
