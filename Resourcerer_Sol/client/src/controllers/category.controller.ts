import type ICategoryDto from '../interfaces/dtos/ICategoryDto';
import * as base from './base.controller';

export async function getAllCategories() {
    const data = await base.sendGet('/categories/all', 'Fetching categories...');
    return data as ICategoryDto[];
}

