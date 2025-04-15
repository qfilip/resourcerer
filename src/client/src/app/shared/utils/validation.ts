export class Validation {
  static notNull = (x: unknown) => x !== null;
  static minLength = (len: number) => (x: string) => this.notNull(x) && x.length >= len;
  static maxLength = (len: number) => (x: string) => this.notNull(x) && x.length <= len;
  static min = (lowEnd: number) => (x: number) => this.notNull(x) && x >= lowEnd;
  static max = (highEnd: number) => (x: number) => this.notNull(x) && x <= highEnd;

  static optional = <T>(validator: (x: T) => boolean) => (value: T | null | undefined) => !value ? true : validator(value);
}