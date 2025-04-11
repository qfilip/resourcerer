export type Ok<T> = { x?: T };
export type Err = { errors: string[] };
export type Result<T> = Ok<T> & Err;

export class Results {
    static makeResult<T>(errs: string[], data?: T) {
        return errs.length > 0
            ? this.makeErr<T>(errs)
            : this.makeOk<T>(data!);
    }
    
    static makeOk<T>(x: T) {
        const r: Result<T> = {
            x: x,
            errors: [] as string[]
        }
    
        return r;
    }
    
    static makeErr<T>(errors: string[]) {
        const r: Result<T> = {
            x: undefined,
            errors: errors
        }
    
        return r;
    }
}