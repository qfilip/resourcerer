import { Component, Input, OnInit, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import {
    ICategoryDto,
    IItemDto,
    IUnitOfMeasureDto,
    IV1CreateElementItem,
    IV1CreateElementItemFormDataDto,
} from '../../../../models/dtos/interfaces';
import { ItemController } from '../../../../controllers/item.controller';
import { UserService } from '../../../../services/user.service';
import { PopupService } from '../../../../services/popup.service';
@Component({
    selector: 'element-item-form',
    standalone: true,
    imports: [ReactiveFormsModule],
    templateUrl: './element-item-form.component.html',
    styleUrl: './element-item-form.component.css',
})
export class ElementItemFormComponent implements OnInit {
    @Input({ required: true }) formType!: 'create' | 'edit';

    itemController = inject(ItemController);
    userService = inject(UserService);
    popupService = inject(PopupService);

    f = new FormGroup({
        name: new FormControl('', [Validators.required, Validators.minLength(2)]),
        productionTimeSeconds: new FormControl(null, [Validators.required, Validators.min(0)]),
        canExpire: new FormControl(false),
        expirationTimeSeconds:  new FormControl(null, [Validators.min(0)]),
        unitPrice: new FormControl(0, [Validators.required, Validators.min(0)]),
        categoryId: new FormControl(null, [Validators.required]),
        unitOfMeasureId: new FormControl(null, [Validators.required])
    });

    formData: IV1CreateElementItemFormDataDto = {
        companyId: '',
        categories: [],
        unitsOfMeasure: [],
    };

    ngOnInit() {
        this.formType === 'create' ? this.loadForCreate() : this.loadForEdit();
    }

    loadForCreate() {
        const companyId = this.userService.user()!.company.id;
        this.itemController.getCreateElementItemFormData(companyId).subscribe({
            next: (x) => (this.formData = x),
        });
    }

    loadForEdit() {
        console.log('editing');
        this.f.controls.name.errors
    }

    onSubmit(ev: Event) {
        ev.preventDefault();
        console.log(this.f.value.name);
        console.log(this.f.value.categoryId);
    }

    createItem() {
        // this.form
    }
}
