import type IDtoBase from "./IDtoBase";
import type IElementDto from "./IElementDto";
import type { IExcerptDto } from "./IExcerptDto";

export default interface UnitOfMeasureDto extends IDtoBase {
    name: string;
    symbol: string;
    excerpts: IExcerptDto[];
    elements: IElementDto[];
}