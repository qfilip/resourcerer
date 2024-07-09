import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, signal } from '@angular/core';
import { ElementItemFormComponent } from "../element-item-form/element-item-form.component";
import { CompositeItemFormComponent } from "../composite-item-form/composite-item-form.component";

@Component({
    selector: 'item-form',
    standalone: true,
    templateUrl: './item-form.component.html',
    styleUrl: './item-form.component.css',
    imports: [CommonModule, ElementItemFormComponent, CompositeItemFormComponent]
})
export class CreateItemComponent {
  @Input({ required: true }) formType!: 'create' | 'edit';
  @Output() onSubmitted = new EventEmitter();
  itemType = signal<'element' | 'composite'>('element');

  onItemTypeChanged(ev: any) {
    const value = ev.target.value as 'element' | 'composite';
    this.itemType.set(value);
  }
}
