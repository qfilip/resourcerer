import type IEntityBase from "./IEntityBase";

export default interface IPrice extends IEntityBase {
    value: number;
    compositeId: string;
}