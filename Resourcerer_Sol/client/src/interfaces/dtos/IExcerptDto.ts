import type ICompositeDto from "./ICompositeDto";
import type IDtoBase from "./IDtoBase";
import type IElementDto from "./IElementDto";

export interface IExcerptDto extends IDtoBase {
    compositeId: string;
    elementId: string;
    composite?: ICompositeDto;
    element?: IElementDto;
    quantity: number;
}