import type ICompositeDto from "./ICompositeDto";
import type IDtoBase from "./IDtoBase";
import type IPriceDto from "./IPriceDto";

export default interface ICompositeSoldEventDto extends IDtoBase {
    compositeId: string;
    priceId: string;
    composite?: ICompositeDto;
    price?: IPriceDto;
}