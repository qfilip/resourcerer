import { Component, computed, inject, input } from '@angular/core';
import { ICategoryDto } from '../../../../shared/dtos/interfaces';
import { CategoryService } from '../../services/category.service';
import { CommonModule } from '@angular/common';
import { DetailsComponent } from "../../../../shared/features/common-ui/components/details/details.component";

@Component({
  standalone: true,
  selector: 'app-category',
  imports: [CommonModule, DetailsComponent],
  templateUrl: './category.component.html',
  styleUrl: './category.component.css'
})
export class CategoryComponent {
  private categoryService = inject(CategoryService);
  
  $category = input.required<ICategoryDto>();
  $selected = computed(() => {
    const s = this.categoryService.$selectedCategory();
    const x = this.$category();
    
    return s?.id === x.id;
  });

  selectCategory = () => this.categoryService.selectCategory(this.$category());
}
