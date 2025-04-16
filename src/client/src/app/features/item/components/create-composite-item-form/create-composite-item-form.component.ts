import { Component, computed, inject, OnInit, output, signal } from '@angular/core';
import { IItemDto, IV1CreateCompositeItem, IV1ElementItemFormData } from '../../../../shared/dtos/interfaces';
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
  $formData = signal<IV1CreateCompositeItem | null>(null);
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
  
  ngOnInit(): void {
    this.items = this.itemService.$items();
    if(this.items.length == 0) {
      this.onFormDataError.emit(['No items found to create a composite']);
      return;
    }
  }

  
}
