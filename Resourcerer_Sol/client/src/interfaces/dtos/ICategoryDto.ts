import type ICompositeDto from "./ICompositeDto";
import type IDtoBase from "./IDtoBase";
import type IElementDto from "./IElementDto";

export default interface ICategoryDto extends IDtoBase {
    name: string;
    parentCategoryId?: string;
    parentCategory?: ICategoryDto;
    childCategories: ICategoryDto[];
    composites: ICompositeDto[];
    elements: IElementDto[];
}