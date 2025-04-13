import { Component, effect, inject, OnInit, output } from '@angular/core';
import { CategoryService } from '../../services/category.service';
import { ICategoryDto, IV1CreateCategory } from '../../../../shared/dtos/interfaces';
import { PopupService } from '../../../../shared/features/common-ui/services/popup.service';

@Component({
  selector: 'app-create-category-form',
  imports: [],
  standalone: true,
  templateUrl: './create-category-form.component.html',
  styleUrl: './create-category-form.component.css'
})
export class CreateCategoryFormComponent implements OnInit {
  private popup = inject(PopupService);
  private categoryService = inject(CategoryService);
  onCategoryCreated = output();
  categories: {id?: string, name: string}[] = [
    {id: undefined, name: 'none' }
  ];
  selected: {id?: string, name: string} = {id: undefined, name: 'none' };
  
  private request!: IV1CreateCategory;

  ngOnInit(): void {
    const xs = this.categoryService.$categories();
    this.categories = this.categories.concat(xs);
    this.request = { name: '', parentCategoryId: undefined, companyId: '' };
  }

  onNameChanged(name: string) {
    this.request.name = name;
  }

  onParentCategorySelected(parentId?: string) {
    this.selected = this.categories.find(x => x.id === parentId)!;
    this.request.parentCategoryId = this.selected.id;
  }

  submit(ev: Event) {
    ev.preventDefault();

    const notEmpty = (x: string) => x && x.length > 0;
    
    if(!notEmpty(this.request.name)) {
      this.popup.warn('Category must have a name');
      return;
    }
    
    this.categoryService.createCategory(this.request)
      .subscribe({ next: () => {
        this.popup.ok('Category created');
        this.onCategoryCreated.emit();
      }
    });
  }
}
