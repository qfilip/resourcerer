import type { Record } from 'pocketbase';
import type ICategory from '../interfaces/dbModels/ICategory';
import * as pb from '../services/pocketbase.service';
import * as loaderService from '../services/commonUi/loader.service';

const coll = 'categories';

const recordToDbModel = (x: Record) => {
    const category: ICategory = {
        id: x.id,
        name: x.name,
        parentCategoryId: x.parentCategoryId,
        created: new Date(x.created),
        updated: new Date(x.updated)
    }
    
    return category;
}

export async function getCategories() {
    const promiseFn = () => pb.database.collection(coll).getFullList();
    const message = 'Fetching categories...';
    const data = await handlePromise(promiseFn, message);
    
    return data.map(recordToDbModel);
}

async function handlePromise<T>(fn: () => Promise<T>, message?: string) {
    loaderService.show(message);
    try {
        const data = await fn();
        loaderService.hide();
        return data;
    }
    catch(error) {
        loaderService.hide();
        console.log(error);
    }
}