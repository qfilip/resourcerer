import type { ICategoryDto } from '../interfaces/dtos/interfaces';
import { http } from './base.controller';

export async function getAllCategories() {
    return http.get('/categories')
    .then(x => x.data as ICategoryDto[]);
}

export function removeCategory(x: ICategoryDto) {
    return http.post('/categories/remove', x)
        .then(x => x.data as string);
}

