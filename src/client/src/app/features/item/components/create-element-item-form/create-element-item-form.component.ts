import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { IV1CreateElementItemFormDataDto, IV1CreateElementItem } from '../../../../shared/dtos/interfaces';
import { UserService } from '../../../user/services/user.service';
import { ItemService } from '../../services/item.service';
import { FormObject, FormObjectControl } from '../../../../shared/utils/forms';
import { Validation } from '../../../../shared/utils/validation';
import { FormErrorComponent } from "../../../../shared/features/common-ui/components/form-error/form-error.component";

@Component({
  selector: 'app-create-element-item-form',
  standalone: true,
  imports: [ReactiveFormsModule, FormErrorComponent],
  templateUrl: './create-element-item-form.component.html',
  styleUrl: './create-element-item-form.component.css'
})
export class CreateElementItemFormComponent {
  @Input({ required: true }) formType!: 'create' | 'edit';
  @Output() onSubmitted = new EventEmitter();
  
  private itemService = inject(ItemService);
  userService = inject(UserService);

  form = new FormObject({
    name: new FormObjectControl({
      value: '',
      validators: [
        { fn: Validation.notNull, error: 'Required' },
        { fn: Validation.minLength(2), error: 'Must be minimum 2 characters long' },
      ]
    }),
    productionPrice: new FormObjectControl({
      value: 0,
      validators: [
        { fn: Validation.notNull, error: 'Required' },
        { fn: Validation.min(0), error: 'Must be 0 or above' },
      ]
    }),
    productionTimeSeconds: new FormObjectControl({
      value: 0,
      validators: [
        { fn: Validation.notNull, error: 'Required' },
        { fn: Validation.min(0), error: 'Must be 0 or above' },
      ]
    }),
    canExpire: new FormObjectControl({
      value: false,
      validators: []
    }),
    expirationTimeSeconds: new FormObjectControl({
      value: null,
      validators: [{ fn: Validation.optional(Validation.min(0)), error: 'Must be 0 or above' },]
    }),
    unitPrice: new FormObjectControl({
      value: 0,
      validators: [
        { fn: Validation.notNull, error: 'Required' },
        { fn: Validation.min(0), error: 'Must be 0 or above' },
      ]
    }),
    categoryId: new FormObjectControl({
      value: null,
      validators: [{ fn: Validation.notNull, error: 'Required' }]
    }),
    unitOfMeasureId: new FormObjectControl({
      value: null,
      validators: [{ fn: Validation.notNull, error: 'Required' }]
    }),
  })

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
      name: this.form.controls.name.data.value!,
      productionPrice: this.form.controls.productionPrice.data.value!,
      productionTimeSeconds: this.form.controls.productionTimeSeconds.data.value!,
      expirationTimeSeconds: this.form.controls.expirationTimeSeconds.data.value,
      unitPrice: this.form.controls.unitPrice.data.value!,
      categoryId: this.form.controls.categoryId.data.value!,
      unitOfMeasureId: this.form.controls.unitOfMeasureId.data.value!
    }

    return dto;
  }
}
