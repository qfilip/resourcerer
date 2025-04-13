import { Component, inject, OnInit, output, signal } from '@angular/core';
import { CategoryService } from '../../services/category.service';
import { ICategoryDto, IV1CreateCategory } from '../../../../shared/dtos/interfaces';
import { PopupService } from '../../../../shared/features/common-ui/services/popup.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-create-category-form',
  imports: [CommonModule],
  standalone: true,
  templateUrl: './create-category-form.component.html',
  styleUrl: './create-category-form.component.css'
})
export class CategoryFormComponent implements OnInit {
  private popup = inject(PopupService);
  private categoryService = inject(CategoryService);

  $formData = signal<IV1CreateCategory>({
    id: '',
    name: '',
    companyId: '',
    parentCategoryId: undefined
  } as IV1CreateCategory);

  onSubmitDone = output();

  categories: ICategoryDto[] = [];
  selected: ICategoryDto | null = null;

  ngOnInit(): void {
    this.categories = this.categoryService.$categories();
    this.selected = this.categories[0];
  }

  onNameChanged(name: string) {
    this.$formData.update(x => ({ ...x, name: name } as ICategoryDto));
  }

  onHasParentChanged(checked: boolean) {
    if (checked && this.selected === null) {
      this.popup.warn('No category exists to be a parent');
    }
    this.$formData.update(x => ({ ...x, parentCategoryId: this.selected?.id } as ICategoryDto));
  }

  onParentCategorySelected(parentId?: string) {
    this.selected = this.categories.find(x => x.id === parentId)!;
    this.$formData.update(x => ({ ...x, parentCategoryId: this.selected?.id } as ICategoryDto));
  }

  submit(ev: Event) {
    ev.preventDefault();
    const formData = this.$formData()!;
    const notEmpty = (x: string) => x && x.length > 0;

    if (!notEmpty(formData.name)) {
      this.popup.warn('Category must have a name');
      return;
    }

    this.categoryService.createCategory(formData)
      .subscribe({
        next: () => {
          this.popup.ok('Category created');
          this.onSubmitDone.emit();
        }
      });
  }
}
