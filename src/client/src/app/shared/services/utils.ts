import { PopupService } from "../features/common-ui/services/popup.service";

export class Utils {
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
}