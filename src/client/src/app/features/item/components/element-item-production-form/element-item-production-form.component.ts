import { Component, inject, input } from '@angular/core';
import { IItemDto, IV1CreateElementItemProductionOrderCommand } from '../../../../shared/dtos/interfaces';
import { ItemService } from '../../services/item.service';
import { DialogService } from '../../../../shared/features/common-ui/services/dialog.service';
import { FormErrorComponent } from "../../../../shared/features/common-ui/components/form-error/form-error.component";
import { FormObject, FormObjectControl } from '../../../../shared/utils/forms';
import { Validation } from '../../../../shared/utils/validation';
import { UserService } from '../../../user/services/user.service';

@Component({
  selector: 'app-element-item-production-form',
  imports: [FormErrorComponent],
  templateUrl: './element-item-production-form.component.html',
  styleUrl: './element-item-production-form.component.css'
})
export class ElementItemProductionFormComponent {
  $item = input.required<IItemDto>();
  private itemService = inject(ItemService);
  private userService = inject(UserService);
  private dialogService = inject(DialogService);

  form = new FormObject({
    quantity: new FormObjectControl<number | null>({
      value: null,
      validators: [
        { fn: Validation.notNull, error: 'Required' },
        { fn: Validation.aboveZero, error: 'Must be larger than 0' }
      ]
    }),
    instantProduction: new FormObjectControl<boolean>({
      value: false,
      validators: []
    }),
    desiredProductionStartTime: new FormObjectControl<Date | null>({
      value: null,
      validators: [{ fn: Validation.notNull, error: 'Required' }]
    }),
    reason: new FormObjectControl<string | null>({
      value: null,
      validators: []
    })
  });

  onSubmit(e: Event) {
    e.preventDefault();
    if(!this.form.valid) return;
    
    this.dialogService.openCheck(
      'Start production?',
      () => this.produceElement()
    );
  }

  private produceElement() {
    const user = this.userService.$user()!;
    const item = this.$item();
    const cs = this.form.controls;
    
    const cmd: IV1CreateElementItemProductionOrderCommand = {
      itemId: item.id,
      companyId: user.company.id,
      quantity: cs.quantity.data.value,
      instantProduction: cs.instantProduction.data.value,
      desiredProductionStartTime: cs.desiredProductionStartTime.data.value,
      reason: cs.reason.data.value
    }

    this.itemService.produceElement(cmd).subscribe();
  }
}
