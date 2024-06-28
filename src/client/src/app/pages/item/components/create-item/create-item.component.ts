import { CommonModule } from '@angular/common';
import { Component, Input, signal } from '@angular/core';
import { ElementItemFormComponent } from "../element-item-form/element-item-form.component";
import { CompositeItemFormComponent } from "../composite-item-form/composite-item-form.component";

@Component({
    selector: 'create-item',
    standalone: true,
    templateUrl: './create-item.component.html',
    styleUrl: './create-item.component.css',
    imports: [CommonModule, ElementItemFormComponent, CompositeItemFormComponent]
})
export class CreateItemComponent {
  @Input({ required: true }) formType!: 'create' | 'edit';
  itemType = signal<'element' | 'composite'>('element');

  onItemTypeChanged(ev: any) {
    const value = ev.target.value as 'element' | 'composite';
    this.itemType.set(value);
  }

  onSubmit(ev: Event) {
    ev.preventDefault();
  }
}
