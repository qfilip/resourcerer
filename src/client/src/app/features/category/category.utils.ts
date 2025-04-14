import { ICategoryDto } from "../../shared/dtos/interfaces";

export class CategoryUtils {
    
    static mapTree(xs: ICategoryDto[]) {
        const mapDto = (current: ICategoryDto, all: ICategoryDto[]) => {
            const children = all.filter(x => x.parentCategoryId === current.id);
            return {...current, childCategories: children } as ICategoryDto;
        }

        return xs
            .filter(x => !x.parentCategoryId)
            .map(x => mapDto(x, xs));
    }
}