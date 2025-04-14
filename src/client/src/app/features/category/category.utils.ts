import { ICategoryDto } from "../../shared/dtos/interfaces";

export class CategoryUtils {
    
    static mapTree(xs: ICategoryDto[]) {
        const mapChildren = (current: ICategoryDto, all: ICategoryDto[]): ICategoryDto => {
            const children = all
                .filter(x => x.parentCategoryId === current.id)
                .map(x => mapChildren(x, all));
            
            return {...current, childCategories: children } as ICategoryDto;
        }

        return xs
            .filter(x => !x.parentCategoryId)
            .map(x => mapChildren(x, xs));
    }

    static flattenTree(xs: ICategoryDto[]) {
        const list: ICategoryDto[] = [];
        const getChildren = (c: ICategoryDto) => {
            list.push(c);
            c.childCategories.forEach(x => getChildren(x));
        }
        xs.forEach(x => getChildren(x));
        
        return list;
    }
}