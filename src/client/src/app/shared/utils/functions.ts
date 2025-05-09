import { FormGroup, ValidationErrors } from "@angular/forms";
import { PopupService } from "../features/common-ui/services/popup.service";
import { IEntityDto } from "../dtos/interfaces";
import { WritableSignal } from "@angular/core";

export class Functions {
    static makeId = () => Math.random().toString(16).substr(2, 8);

    static printErrors(service: PopupService, errors: string[]) {
        errors.forEach(x => {
            service.push({
                color: 'warn',
                header: 'Validation failed',
                text: x 
            });
        })
    }

    static deepClone<T>(x: T) {
        if(typeof x !== 'object' || x === null) return x;
    
        const clone: any = Array.isArray(x) ? [] : {};
    
        for(let key in x) {
          const value = x[key];
          clone[key] = this.deepClone(value);
        }
    
        return clone as T;
    }

    static timeSort = (a: Date, b: Date) => 
        new Date(a).getTime() > new Date(b).getTime() ? 1 : -1

    static getFormValidationErrors(form: FormGroup) {
      const result: any = [];
      Object.keys(form.controls).forEach(key => {
    
        const controlErrors: ValidationErrors = form.get(key)!.errors!;
        if (controlErrors) {
          Object.keys(controlErrors).forEach(keyError => {
            result.push({
              'control': key,
              'error': keyError,
              'value': controlErrors[keyError]
            });
          });
        }
      });
    
      return result;
    }

    static getReducer<T extends IEntityDto>($signal: WritableSignal<T[]>, reducerError: string) {
      return (all?: T[], created?: T, updated?: T, removed?: T) => {
        if(all)
          $signal.set(all);
        else if(created)
          $signal.update(xs => xs.concat(created));
        else if(updated)
          $signal.update(xs => xs.filter(x => x.id !== updated.id).concat(updated));
        else if(removed)
          $signal.update(xs => xs.filter(x => x.id !== removed.id));
        else
          throw reducerError;
      }
    }
}