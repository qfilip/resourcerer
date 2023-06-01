import type IEntityBase from "./IEntityBase";

export default interface IExcerpt extends IEntityBase {
    compositeId: string;
    elementId: string;
    quantity: number;
}