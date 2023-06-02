import type ICategoryDto from "./ICategoryDto";
import type IDtoBase from "./IDtoBase";
import type IElementPurchasedEventDto from "./IElementPurchasedEventDto";
import type { IExcerptDto } from "./IExcerptDto";
import type IUnitOfMeasureDto from "./IUnitOfMeasureDto";

export default interface IElementDto extends IDtoBase {
    name: string;
    categoryId: string;
    unitOfMeasureId: string;
    category?: ICategoryDto;
    unitOfMeasure?: IUnitOfMeasureDto;
    excerpts: IExcerptDto[];
    elementPurchasedEvents: IElementPurchasedEventDto[];
}