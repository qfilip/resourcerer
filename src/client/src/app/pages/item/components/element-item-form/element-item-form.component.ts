import { Component, Input, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ICategoryDto, IItemDto, IUnitOfMeasureDto } from '../../../../models/dtos/interfaces';
import { ItemController } from '../../../../controllers/item.controller';
import { UserService } from '../../../../services/user.service';

@Component({
  selector: 'element-item-form',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './element-item-form.component.html',
  styleUrl: './element-item-form.component.css'
})
export class ElementItemFormComponent implements OnInit {
  @Input({ required: true }) formType!: 'create' | 'edit';

  itemController = inject(ItemController);
  userService = inject(UserService);

  elementItem = {
    name: '',
    productionTimeSeconds: 0,
    expirationTimeSeconds: 0
  } as IItemDto;
  categories: ICategoryDto[] = [];
  unitsOfMeasure: IUnitOfMeasureDto[] = [];
  unitPrice = 0;

  ngOnInit() {
    this.formType === 'create'
      ? this.loadForCreate()
      : this.loadForEdit();
  }

  loadForCreate() {
    const companyId = this.userService.user()!.companyId;
    this.itemController.getCreateElementItemFormData(companyId)
      .subscribe({
        next: (x) => {
          this.categories = [... x.categories];
          this.unitsOfMeasure = [... x.unitsOfMeasure];
        }
      });
  }

  loadForEdit() {
    console.log('editing');
  }

  onSubmit(ev: Event) {
    ev.preventDefault();
  }
}
