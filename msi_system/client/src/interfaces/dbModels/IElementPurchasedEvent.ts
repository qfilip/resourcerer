import type IEntityBase from "./IEntityBase";

export default interface IElementPurchasedEvent extends IEntityBase {
    elementId: string;
    numOfUnits: number;
    unitPrice: number;
}