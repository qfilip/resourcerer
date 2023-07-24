import type { ICategoryDto } from '../interfaces/dtos/interfaces';
import { http } from './base.controller';

export async function getAllCategories() {
    return http.get('/categories')
    .then(x => x.data as ICategoryDto[]);
}

