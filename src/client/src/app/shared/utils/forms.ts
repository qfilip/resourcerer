export type FormObjectControlData<T> = {
  value: T,
  validators: { fn: (v: T) => boolean, error: string }[],
  skipValidation?: boolean,
  errors?: string[]
}

export class FormObjectControl<T> {
  data: FormObjectControlData<T>;
  submitted = false;
  constructor(data: FormObjectControlData<T>) {
    this.data = data;
  }

  setValue(value: T, skipValidation?: boolean) {
    this.data.value = value;
    this.data.skipValidation = skipValidation;
    this.data.errors = this.errors;
  }

  get errors() {
    return this.data.validators
      .map(x => {
        const error = x.fn(this.data.value) ? null : x.error
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
    for(const key in this._controls) {
      this._controls[key].submitted = true;
      valid = valid && this._controls[key].errors.length === 0;
    }

    return valid;
  }

  get controls() {
    return this._controls;
  }
}