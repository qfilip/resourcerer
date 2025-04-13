import { Component, inject, OnInit, Signal, signal } from '@angular/core';
import { CategoryService } from '../../services/category.service';
import { CategoryListComponent } from "../../components/category-list/category-list.component";
import { CategoryFormComponent } from "../../components/create-category-form/create-category-form.component";
import { ICategoryDto } from '../../../../shared/dtos/interfaces';

@Component({
  standalone: true,
  selector: 'app-category-page',
  imports: [CategoryListComponent, CategoryFormComponent],
  templateUrl: './category.page.html',
  styleUrl: './category.page.css'
})
export class CategoryPage implements OnInit {
  private categoryService = inject(CategoryService);
  $updateItem = signal<ICategoryDto | null>(null);
  $component = signal<'createForm' | 'updateForm' | null>(null);
  
  ngOnInit(): void {
    this.categoryService.getAllCompanyCategories();
  }

  showComponent(x: 'createForm' | 'updateForm' | null, data: ICategoryDto | null) {
    this.$component.set(x);
    this.$updateItem.set(data);
  }
}
