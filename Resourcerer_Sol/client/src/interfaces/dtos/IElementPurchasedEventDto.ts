import type IDtoBase from "./IDtoBase";
import type IElementDto from "./IElementDto";

export default interface IElementPurchasedEventDto extends IDtoBase {
    elementId: string;
    numOfUnits: number;
    unitPrice: number;
    element?: IElementDto;
}