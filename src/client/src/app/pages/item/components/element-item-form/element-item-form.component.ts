import { Component, Input, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ICategoryDto, IItemDto, IUnitOfMeasureDto } from '../../../../models/dtos/interfaces';

@Component({
  selector: 'element-item-form',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './element-item-form.component.html',
  styleUrl: './element-item-form.component.css'
})
export class ElementItemFormComponent implements OnInit {
  @Input({ required: true }) formType!: 'create' | 'edit';

  elementItem = {
    name: '',
    productionTimeSeconds: 0,
    expirationTimeSeconds: 0,
    category: {} as ICategoryDto,
    unitOfMeasure: {} as IUnitOfMeasureDto
  } as IItemDto;
  unitPrice = 0;

  ngOnInit() {
    this.formType === 'create'
      ? this.loadForCreate()
      : this.loadForEdit();
  }

  loadForCreate() {
    console.log('creating');
  }

  loadForEdit() {
    console.log('editing');
  }

  onSubmit(ev: Event) {
    ev.preventDefault();
  }
}
