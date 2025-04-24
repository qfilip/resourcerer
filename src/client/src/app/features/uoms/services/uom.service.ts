import { inject, Injectable, signal } from "@angular/core";
import { UomApiService } from "./uom.api.service";
import { IUnitOfMeasureDto, IV1CreateUnitOfMeasure, IV1EditUnitOfMeasure } from "../../../shared/dtos/interfaces";
import { UserService } from "../../user/services/user.service";
import { tap } from "rxjs";
import { Functions } from "../../../shared/utils/functions";

@Injectable({ providedIn: 'root'})
export class UomService {
  private apiService = inject(UomApiService);
  private userService = inject(UserService);

  private _$uoms = signal<IUnitOfMeasureDto[]>([]);
  private _$selectedUom = signal<IUnitOfMeasureDto | null>(null);
  
  $uoms = this._$uoms.asReadonly();
  $selectedUom = this._$selectedUom.asReadonly();

  private reducer = Functions.getReducer<IUnitOfMeasureDto>(this._$uoms, 'UnitOfMeasure reducer failed');

  selectUom = (x: IUnitOfMeasureDto) => this._$selectedUom.set(x);

  getCompanyUnitsOfMeasure() {
    const user = this.userService.$user()!;
    
    this.apiService.getCompanyUnitsOfMeasure(user.company.id)
      .subscribe({
        next: xs => this.runReducers(xs)
      });
  }

  createUnitOfMeasure(dto: IV1CreateUnitOfMeasure) {
    const user = this.userService.$user()!;
    const request: IV1CreateUnitOfMeasure = {
      ...dto,
      companyId: user.company.id
    };

    return this.apiService.createUnitOfMeasure(request)
      .pipe(
        tap(x => this.runReducers(undefined, x))
      );
  }

  editUnitOfMeasure(dto: IV1EditUnitOfMeasure) {
    return this.apiService.editUnitOfMeasure(dto)
      .pipe(
        tap(x => this.runReducers(undefined, undefined, x))
      );
  }

  deleteUnitOfMeasure(dto: IUnitOfMeasureDto) {
    return this.apiService.deleteUnitOfMeasure(dto.id)
      .pipe(
        tap(x => this.runReducers(undefined, undefined, undefined, x))
      );
  }

  private runReducers(all?: IUnitOfMeasureDto[], created?: IUnitOfMeasureDto, updated?: IUnitOfMeasureDto, removed?: IUnitOfMeasureDto) {
    this.reducer(all, created, updated, removed);
    this._$selectedUom.set(null);
  }
}