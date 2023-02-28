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

export async function getElementsOverviews() {
    const category = 'categoryId'
    const excerpt = 'excerpts(elementId)';
    const uom = 'unitOfMeasureId';

    const getData = () => pb.database.collection('elements').getFullList({
        expand: [uom, excerpt, category].join(',')
    });

    const data = await base.handlePromise(getData, 'Fetching elements...');
    
    return {
        elements: data.map(base.mapElement),
        categories: data.map(x => base.mapExpand(x, category, base.mapCategory)).flat(),
        unitsOfMeasure: data.map(x => base.mapExpand(x, uom, base.mapUnitOfMeasure)).flat(),
        excerpts: data.map(x => base.mapExpand(x, excerpt, base.mapExcerpt)).flat()
    }
}

export async function getElementsForTable() {
    const excerpt = 'excerpts(elementId)';
    const purchase = 'elementPurchasedEvents(elementId)';
    const uom = 'unitOfMeasureId';

    const getElementData = () => pb.database.collection('elements').getFullList({
        expand: [purchase, uom, excerpt].join(',')
    });
    
    const elementData = await base.handlePromise(getElementData, 'Fetching elements...');
    
    const elementsRelations = {
        elements: elementData.map(base.mapElement),
        purchaseEvents: elementData.map(x => base.mapExpand(x, purchase, base.mapElementPurchasedEvent)).flat(),
        unitsOfMeasure: elementData.map(x => base.mapExpand(x, uom, base.mapUnitOfMeasure)).flat(),
        excerpts: elementData.map(x => base.mapExpand(x, excerpt, base.mapExcerpt)).flat()
    };

    console.log(elementData)

    return elementData;
}