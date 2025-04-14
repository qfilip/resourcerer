import { Component, inject, output } from '@angular/core';
import { UomService } from '../../services/uom.service';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { IV1CreateUnitOfMeasure } from '../../../../shared/dtos/interfaces';
import { Utils } from '../../../../shared/services/utils';

@Component({
  selector: 'app-create-uom-form',
  imports: [ReactiveFormsModule],
  templateUrl: './create-uom-form.component.html',
  styleUrl: './create-uom-form.component.css'
})
export class CreateUomFormComponent {
  private uomService = inject(UomService);

  onCreated = output();

  form = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.minLength(2)]),
    symbol: new FormControl('', [Validators.required, Validators.minLength(2)])
  });
  
  formSubmitted = false;

  onSubmit(ev: Event) {
    ev.preventDefault();
    console.log(Utils.getFormValidationErrors(this.form));
    this.formSubmitted = true;

    if (!this.form.valid) {
      return;
    }

    const dto = {
      name: this.form.controls.name.value!,
      symbol: this.form.controls.symbol.value!,
    } as IV1CreateUnitOfMeasure;

    this.uomService.createUnitOfMeasure(dto)
      .subscribe({
        next: () => this.onCreated.emit()
      });
  }
}
