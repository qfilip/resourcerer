import type { Record } from 'pocketbase';
import type IElement from '../interfaces/dbModels/IElement';
import type ICategory from '../interfaces/dbModels/ICategory';
import type IEntityBase from '../interfaces/dbModels/IEntityBase';
import * as loaderService from '../services/commonUi/loader.service';
import type IElementPurchasedEvent from '../interfaces/dbModels/IElementPurchasedEvent';
import type IUnitOfMeasure from '../interfaces/dbModels/IUnitOfMeasure';

export function mapEntityBase<T extends IEntityBase>(r: Record, x: T) {
    const entityBase: IEntityBase = {
        id: r.id,
        created: new Date(r.created),
        updated: new Date(r.updated),
    }

    return Object.assign(x, entityBase) as T;
}

export function mapElement(x: Record) {
    let entity: Omit<IElement, keyof IEntityBase> = {
        name: x.name,
        categoryId: x.categoryId,
        unitOfMeasureId: x.unitOfMeasureId
    };
    
    return mapEntityBase(x, entity as IElement);
}

export function mapCategory(x: Record) {
    let entity: Omit<ICategory, keyof IEntityBase> = {
        name: x.name,
        parentCategoryId: x.parentCategoryId,
    };
    
    return mapEntityBase(x, entity as ICategory);
}

export function mapUnitOfMeasure(x: Record) {
    let entity: Omit<IUnitOfMeasure, keyof IEntityBase> = {
        name: x.name,
        symbol: x.symbol
    };
    
    return mapEntityBase(x, entity as IUnitOfMeasure);
}

export function mapElementPurchasedEvent(x: Record) {
    let entity: Omit<IElementPurchasedEvent, keyof IEntityBase> = {
        elementId: x.elementId,
        numOfUnits: x.numOfUnits,
        unitPrice: x.unitPrice
    };
    
    return mapEntityBase(x, entity as IElementPurchasedEvent);
}

export function mapExpand<T extends IEntityBase>(r: Record, expandKey: string, mapper: (x: Record) => T) {
    const data = r.expand[expandKey];
    
    if(Array.isArray(data)) {
        return data.map(mapper);
    }
    else {
        return [mapper(data)];
    }
}

export async function handlePromise<T>(fn: () => Promise<T>, message?: string) {
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