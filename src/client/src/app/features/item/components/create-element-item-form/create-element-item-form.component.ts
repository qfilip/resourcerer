import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { IV1CreateElementItemFormDataDto, IV1CreateElementItem } from '../../../../shared/dtos/interfaces';
import { UserService } from '../../../user/services/user.service';
import { ItemService } from '../../services/item.service';

@Component({
  selector: 'app-create-element-item-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './create-element-item-form.component.html',
  styleUrl: './create-element-item-form.component.css'
})
export class CreateElementItemFormComponent {
  @Input({ required: true }) formType!: 'create' | 'edit';
  @Output() onSubmitted = new EventEmitter();
  
  private itemService = inject(ItemService);
  userService = inject(UserService);

  form = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.minLength(3)]),
    productionPrice: new FormControl(0, [Validators.required, Validators.min(0)]),
    productionTimeSeconds: new FormControl(0, [Validators.required, Validators.min(0)]),
    canExpire: new FormControl(false),
    expirationTimeSeconds: new FormControl(null, [Validators.min(0)]),
    unitPrice: new FormControl(0, [Validators.required, Validators.min(0)]),
    categoryId: new FormControl(null, [Validators.required]),
    unitOfMeasureId: new FormControl(null, [Validators.required])
  });
  formSubmitted = false;

  formData: IV1CreateElementItemFormDataDto = {
    companyId: '',
    categories: [],
    unitsOfMeasure: [],
  };

  ngOnInit() {
    this.formType === 'create' ? this.loadForCreate() : this.loadForEdit();
  }

  onSubmit(ev: Event) {
    ev.preventDefault();
    this.formSubmitted = true;

    if (!this.form.valid) {
      return;
    }

    this.formType === 'create' ? this.createItem() : this.editItem();
  }

  loadForCreate() {
    this.itemService.getCreateElementItemFormData()
      .subscribe({
        next: x => this.formData = x
      });
  }

  createItem() {
    const dto = this.mapDtoFromForm();

    this.itemService.createElementItem(dto)
      .subscribe({
        next: _ => this.onSubmitted.emit()
      })
  }

  loadForEdit() {

  }

  editItem() {

  }

  private mapDtoFromForm() {
    const dto: IV1CreateElementItem = {
      name: this.form.controls.name.value!,
      productionPrice: this.form.controls.productionPrice.value!,
      productionTimeSeconds: this.form.controls.productionTimeSeconds.value!,
      expirationTimeSeconds: this.form.controls.expirationTimeSeconds.value ?? undefined,
      unitPrice: this.form.controls.unitPrice.value!,
      categoryId: this.form.controls.categoryId.value!,
      unitOfMeasureId: this.form.controls.unitOfMeasureId.value!
    }

    return dto;
  }
}
