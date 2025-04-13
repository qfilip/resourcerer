import { CommonModule } from '@angular/common';
import { Component, effect, inject, input, output, signal } from '@angular/core';
import { IV1CreateCategory, ICategoryDto, IV1UpdateCategory } from '../../../../shared/dtos/interfaces';
import { PopupService } from '../../../../shared/features/common-ui/services/popup.service';
import { CategoryService } from '../../services/category.service';
import { FormsModule } from '@angular/forms';

@Component({
  standalone: true,
  selector: 'app-update-category-form',
  imports: [CommonModule, FormsModule],
  templateUrl: './update-category-form.component.html',
  styleUrl: './update-category-form.component.css'
})
export class UpdateCategoryFormComponent {
  private popup = inject(PopupService);
  private categoryService = inject(CategoryService);

  $updateItem = input.required<ICategoryDto>();
  $formData = signal<IV1UpdateCategory>({
    categoryId: '',
    newName: '',
    newParentCategoryId: undefined
  });
  onSubmitDone = output();

  categories: ICategoryDto[] = [];
  selected: ICategoryDto | undefined = undefined;

  constructor() {
    effect(() => {
      const item = this.$updateItem();
      this.selected = this.categories.find(x => x.id === item.parentCategoryId);
      this.$formData.set({
        categoryId: item.id,
        newName: item.name,
        newParentCategoryId: item.parentCategoryId
      });
    })
  }

  ngOnInit(): void {
    this.categories = this.categoryService.$categories();
    this.selected = this.categories[0];
  }

  onNameChanged(name: string) {
    this.$formData.update(x => ({ ...x, newName: name }));
  }

  onHasParentChanged(checked: boolean) {
    if (checked && this.selected === null) {
      this.popup.warn('No category exists to be a parent');
    }
    this.$formData.update(x => ({ ...x, newParentCategoryId: this.selected?.id }));
  }

  onParentCategorySelected(parentId?: string) {
    this.selected = this.categories.find(x => x.id === parentId)!;
    this.$formData.update(x => ({ ...x, newParentCategoryId: this.selected?.id }));
  }

  submit(ev: Event) {
    ev.preventDefault();
    const formData = this.$formData()!;
    const notEmpty = (x: string) => x && x.length > 0;

    if (!notEmpty(formData.newName)) {
      this.popup.warn('Category must have a name');
      return;
    }

    this.categoryService.updateCategory(formData)
      .subscribe({
        next: () => {
          this.popup.ok('Category created');
          this.onSubmitDone.emit();
        }
      });
  }
}
