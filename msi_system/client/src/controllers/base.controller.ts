import type { Record } from 'pocketbase';
import type IElement from '../interfaces/dbModels/IElement';
import type ICategory from '../interfaces/dbModels/ICategory';
import type IEntityBase from '../interfaces/dbModels/IEntityBase';
import * as loaderService from '../services/commonUi/loader.service';
import type IElementPurchasedEvent from '../interfaces/dbModels/IElementPurchasedEvent';
import type IUnitOfMeasure from '../interfaces/dbModels/IUnitOfMeasure';
import type IExcerpt from '../interfaces/dbModels/IExcerpt';
import type IComposite from '../interfaces/dbModels/IComposite';
import type IPrice from '../interfaces/dbModels/IPrice';
import type ICompositeSoldEvent from '../interfaces/dbModels/ICompositeSoldEvent';

type PartialEntity<T extends IEntityBase> = Omit<T, keyof IEntityBase>;

export function mapEntityBase<T extends IEntityBase>(r: Record, x: PartialEntity<T>): T {
    const entityBase: IEntityBase = {
        id: r.id,
        created: new Date(r.created),
        updated: new Date(r.updated),
    }

    return Object.assign(x, entityBase) as T;
}

export function mapCategory(x: Record) {
    const entity: PartialEntity<ICategory> = {
        name: x.name,
        parentCategoryId: x.parentCategoryId,
    };
    
    return mapEntityBase(x, entity);
}

export function mapComposite(x: Record) {
    const entity: PartialEntity<IComposite> = {
        name: x.name,
        categoryId: x.categoryId
    };
    
    return mapEntityBase(x, entity);
}

export function mapCompositeSoldEvent(x: Record) {
    const entity: PartialEntity<ICompositeSoldEvent> = {
        compositeId: x.compositeId,
        priceId: x.priceId
    };
    
    return mapEntityBase(x, entity);
}

export function mapElement(x: Record) {
    const entity: PartialEntity<IElement> = {
        name: x.name,
        categoryId: x.categoryId,
        unitOfMeasureId: x.unitOfMeasureId
    };
    
    return mapEntityBase(x, entity);
}

export function mapElementPurchasedEvent(x: Record) {
    const entity: PartialEntity<IElementPurchasedEvent> = {
        elementId: x.elementId,
        numOfUnits: x.numOfUnits,
        unitPrice: x.unitPrice
    };
    
    return mapEntityBase(x, entity);
}

export function mapExcerpt(x: Record) {
    const entity: PartialEntity<IExcerpt> = {
        compositeId: x.compositeId,
        elementId: x.elementId,
        quantity: x.quantity
    };
    
    return mapEntityBase(x, entity);
}

export function mapPrice(x: Record) {
    const entity: PartialEntity<IPrice> = {
        compositeId: x.compositeId,
        value: x.value
    };
    
    return mapEntityBase(x, entity);
}

export function mapUnitOfMeasure(x: Record) {
    const entity: PartialEntity<IUnitOfMeasure> = {
        name: x.name,
        symbol: x.symbol
    };
    
    return mapEntityBase(x, entity);
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

export async function handlePromise<T>(dbCall: () => Promise<T>, message?: string) {
    loaderService.show(message);
    try {
        const data = await dbCall();
        loaderService.hide();
        return data;
    }
    catch(error) {
        loaderService.hide();
        console.log(error);
    }
}