import { Component, computed, inject, OnInit, Signal, signal } from '@angular/core';
import { CategoryService } from '../../services/category.service';
import { CategoryListComponent } from "../../components/category-list/category-list.component";
import { CategoryFormComponent } from "../../components/create-category-form/create-category-form.component";
import { ICategoryDto } from '../../../../shared/dtos/interfaces';
import { UpdateCategoryFormComponent } from "../../components/update-category-form/update-category-form.component";
import { DialogService } from '../../../../shared/features/common-ui/services/dialog.service';
import { PopupService } from '../../../../shared/features/common-ui/services/popup.service';

@Component({
  standalone: true,
  selector: 'app-category-page',
  imports: [CategoryListComponent, CategoryFormComponent, UpdateCategoryFormComponent],
  templateUrl: './category.page.html',
  styleUrl: './category.page.css'
})
export class CategoryPage implements OnInit {
  private popup = inject(PopupService);
  private dialogService = inject(DialogService);
  private categoryService = inject(CategoryService);
  
  $selectedCategory = this.categoryService.$selectedCategory;
  $component = signal<'createForm' | 'updateForm' | null>(null);
  
  ngOnInit(): void {
    this.categoryService.getAllCompanyCategories();
  }

  selectTree = (x: ICategoryDto) => this.categoryService.selectCategory(x);

  showComponent(x: 'createForm' | 'updateForm' | null) {
    this.$component.set(x);
  }

  removeCategory(dto: ICategoryDto) {
    this.dialogService.openCheck(
      `Remove category ${dto.name}?`,
      () =>
        this.categoryService.removeCategory(dto)
          .subscribe({ next: () => this.popup.ok('Category removed')})
    );
  }
}
