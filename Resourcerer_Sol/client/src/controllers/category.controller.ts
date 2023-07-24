import { http } from './base.controller';

export async function getAllCategories() {
    return http.get('/categories')
    .then(x => console.log(x.data));
}

