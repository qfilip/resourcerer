import { Component, inject, OnInit, signal } from '@angular/core';
import { CategoryService } from '../../services/category.service';
import { CategoryListComponent } from "../../components/category-list/category-list.component";
import { CreateCategoryFormComponent } from "../../components/create-category-form/create-category-form.component";

@Component({
  standalone: true,
  selector: 'app-category-page',
  imports: [CategoryListComponent, CreateCategoryFormComponent],
  templateUrl: './category.page.html',
  styleUrl: './category.page.css'
})
export class CategoryPage implements OnInit {
  private categoryService = inject(CategoryService);
  $component = signal<'createForm' | null>(null);
  
  ngOnInit(): void {
    this.categoryService.getAllCompanyCategories();
  }

  showComponent = (x: 'createForm' | null) => this.$component.set(x);
}
