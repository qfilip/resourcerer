import type ICategory from '../interfaces/dbModels/ICategory';
import * as base from './base.controller';

export async function getCategories() {
    const url = base.apiUrl + '/categories/all';
    const response = await fetch(url, { method: 'GET', mode: 'no-cors' });
    console.log(response);
    return response.json();
    // const message = 'Fetching categories...';
    // return base.handlePromise(apiCall, message);
}

