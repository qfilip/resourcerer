import { inject, Injectable, signal } from "@angular/core";
import { IInstanceDto } from "../../../shared/dtos/interfaces";
import { InstanceApiService } from "./instance.api.service";
import { tap } from "rxjs";

@Injectable({ providedIn: 'root' })
export class InstanceService {
  private apiService = inject(InstanceApiService);
  
  private _$instances = signal<IInstanceDto[]>([]);
  private _$selectedInstance = signal<IInstanceDto | null>(null);
  
  $instances = this._$instances.asReadonly();
  $selectedInstance = this._$selectedInstance.asReadonly();
  
  setSelected = (x: IInstanceDto) => this._$selectedInstance.set(x);

  getItemInstances(itemId: string) {
    return this.apiService.getItemInstances(itemId)
      .pipe(tap(xs => this._$instances.set(xs)));
  }
}