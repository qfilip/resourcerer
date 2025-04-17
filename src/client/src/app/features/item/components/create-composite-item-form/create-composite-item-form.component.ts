import { Component, computed, inject, OnInit, output, signal } from '@angular/core';
import { IItemDto, IV1CompositeItemFormData, IV1CreateCompositeItem, IV1ElementItemFormData } from '../../../../shared/dtos/interfaces';
import { ItemService } from '../../services/item.service';
import { FormObject, FormObjectControl } from '../../../../shared/utils/forms';
import { Validation } from '../../../../shared/utils/validation';
import { FormErrorComponent } from "../../../../shared/features/common-ui/components/form-error/form-error.component";

@Component({
  selector: 'app-create-composite-item-form',
  imports: [FormErrorComponent],
  templateUrl: './create-composite-item-form.component.html',
  styleUrl: './create-composite-item-form.component.css'
})
export class CreateCompositeItemFormComponent implements OnInit {
  onSubmitDone = output<IItemDto>();
  onFormDataError = output<string[]>();
  
  private itemService = inject(ItemService);
  
  items: IItemDto[] = [];
  itemsToUse: { item: IItemDto, qty: number }[] = [];
  $formData = signal<IV1CompositeItemFormData | null>(null);
  $form = computed(() => {
    const formData = this.$formData();
    if(!formData) return null;

    return new FormObject({
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
        validators: [{ fn: Validation.optional(Validation.min(0)), error: 'Must be 0 or above' }]
      }),
      unitPrice: new FormObjectControl({
        value: 0,
        validators: [
          { fn: Validation.notNull, error: 'Required' },
          { fn: Validation.min(0), error: 'Must be 0 or above' },
        ]
      }),
      itemId: new FormObjectControl({
        value: formData.items[0].id,
        validators: [{ fn: Validation.notNull, error: 'Required' }]
      }),
      categoryId: new FormObjectControl({
        value: formData.categories[0].id,
        validators: [{ fn: Validation.notNull, error: 'Required' }]
      }),
      unitOfMeasureId: new FormObjectControl({
        value: formData.unitsOfMeasure[0].id,
        validators: [{ fn: Validation.notNull, error: 'Required' }]
      }),
    });
  });
  
  ngOnInit() {
    this.itemService.getCompositeItemFormData()
      .subscribe({
        next: x => {
          const errors: string[] = [];
          
          if(x.items.length === 0)
            errors.push('At least 1 item must exist to create element item');

          if(x.categories.length === 0)
            errors.push('At least 1 category must exist to create element item');
          
          if(x.unitsOfMeasure.length === 0)
            errors.push('At least 1 unit of measure must exist to create element item');
          
          errors.length === 0
            ? this.$formData.set(x)
            : this.onFormDataError.emit(errors);
        }
      });
  }

  
}
