import type ICategoryDto from "./ICategoryDto";
import type ICompositeSoldEventDto from "./ICompositeSoldEventDto";
import type IDtoBase from "./IDtoBase";
import type { IExcerptDto } from "./IExcerptDto";
import type IPriceDto from "./IPriceDto";

export default interface ICompositeDto extends IDtoBase {
    name: string;
    categoryId: string;
    category: ICategoryDto | null;
    prices: IPriceDto[];
    excerpts: IExcerptDto[];
    compositeSoldEvents: ICompositeSoldEventDto[];
}