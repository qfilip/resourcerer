import type ICompositeDto from "./ICompositeDto";
import type ICompositeSoldEventDto from "./ICompositeSoldEventDto";
import type IDtoBase from "./IDtoBase";

export default interface IPriceDto extends IDtoBase {
    compositeId: string;
    value: number;
    composite?: ICompositeDto;
    compositeSoldEvents: ICompositeSoldEventDto[];
}