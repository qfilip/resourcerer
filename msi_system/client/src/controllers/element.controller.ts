import type { Record } from 'pocketbase';
import type IElement from '../interfaces/dbModels/IElement';
import * as pb from '../services/pocketbase.service';
import * as base from './base.controller';

const coll = 'elements';

const recordToDbModel = (x: Record) => {
    const entity = base.mapBaseEntity<IElement>(x);
    entity.name = x.name;
    entity.categoryId = x.categoryId;
    entity.unitOfMeasureId = x.unitOfMeasureId;
    
    return entity;
}

export async function getAllElements() {
    const promiseFn = () => pb.database.collection(coll).getFullList();
    const message = 'Fetching elements...';
    const data = await base.handlePromise(promiseFn, message);
    
    return data.map(recordToDbModel);
}

export async function getElementsForTable() {
    const promiseFn = () => pb.database.collection('elements').getFullList({
        expand: 'elementPurchasedEvents(elementId)'
    });
    const message = 'Fetching elements...';
    const data = await base.handlePromise(promiseFn, message);
    console.log(data)
    // return data.map(recordToDbModel);
}