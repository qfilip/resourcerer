export type FormObjectControlData<T> = {
  value: T | null,
  validators: { fn: (v: T) => boolean, error: string }[]
}

export class FormObjectControl<T> {
  private fieldValue: T | null;
  private validators: { fn: (v: T) => boolean, error: string }[];
  private skipValidation: boolean = false;
  
  submitted = false;
  constructor(data: FormObjectControlData<T>) {
    this.fieldValue = data.value;
    this.validators = data.validators;
  }

  setValue(value: T, skipValidation?: boolean) {
    this.fieldValue = value;
    if(skipValidation)
      this.skipValidation = skipValidation;
  }

  get value() {
    return this.fieldValue;
  }

  get errors() {
    if(this.skipValidation) return [];
    
    return this.validators
      .map(x => {
        const error = x.fn(this.fieldValue!) ? null : x.error
        return this.submitted ? error : null;
      })
      .filter(x => x !== null);
  }
}

export class FormObject<TControl> {
  private _controls;
  constructor(controls: { [K in keyof TControl]: FormObjectControl<any> }) {
    this._controls = controls;
  }

  get valid() {
    let valid = true;
    for(const key in this.controls) {
      this.controls[key].submitted = true;
      valid = valid && this.controls[key].errors.length === 0;
    }

    return valid;
  }

  // allow access to form controls
  get controls() {
    return this._controls;
  }
}