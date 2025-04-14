export class Validation {
  static notNull = (x: unknown) => x !== null;
  static minLength = (len: number) => (x: string) => this.notNull(x) && x.length >= len;
  static maxLength = (len: number) => (x: string) => this.notNull(x) && x.length <= len;
}