import type { Record } from 'pocketbase';
import type ICategory from '../interfaces/dbModels/ICategory';
import * as pb from '../services/pocketbase.service';
import * as base from './base.controller';

const coll = 'categories';

const recordToDbModel = (x: Record) => {
    const entity = base.mapBaseEntity<ICategory>(x);
    entity.name = x.name;
    entity.parentCategoryId = x.parentCategoryId;
    
    return entity;
}

export async function getCategories() {
    const promiseFn = () => pb.database.collection(coll).getFullList();
    const message = 'Fetching categories...';
    const data = await base.handlePromise(promiseFn, message);
    
    return data.map(recordToDbModel);
}

