import type { Record } from 'pocketbase';
import type IElement from '../interfaces/dbModels/IElement';
import * as pb from '../services/pocketbase.service';
import * as base from './base.controller';

const coll = 'elements';

export async function getAllElements() {
    const promiseFn = () => pb.database.collection(coll).getFullList();
    const message = 'Fetching elements...';
    const data = await base.handlePromise(promiseFn, message);
    
    return data.map(base.mapElement);
}

export async function getElementsForTable() {
    const purchase = 'elementPurchasedEvents(elementId)';
    const uom = 'unitOfMeasureId';
    const expandKey = [purchase, uom].join(',');

    const promiseFn = () => pb.database.collection('elements').getFullList({
        expand: expandKey
    });
    
    const data = await base.handlePromise(promiseFn, 'Fetching elements...');
    
    return {
        elements: data.map(base.mapElement),
        purchaseEvents: data.map(x => base.mapExpand(x, purchase, base.mapElementPurchasedEvent)).flat(),
        unitsOfMeasure: data.map(x => base.mapExpand(x, uom, base.mapUnitOfMeasure)).flat()
    };
}