import type ICategory from "./interfaces/dbModels/ICategory";
import type IComposite from "./interfaces/dbModels/IComposite";
import type ICompositeSoldEvent from "./interfaces/dbModels/ICompositeSoldEvent";
import type IElement from "./interfaces/dbModels/IElement";
import type IElementPurchasedEvent from "./interfaces/dbModels/IElementPurchasedEvent";
import type IEntityBase from "./interfaces/dbModels/IEntityBase";
import type IExcerpt from "./interfaces/dbModels/IExcerpt";
import type IPrice from "./interfaces/dbModels/IPrice";
import type IUnitOfMeasure from "./interfaces/dbModels/IUnitOfMeasure";

import PocketBase from 'pocketbase';


export interface IDatabase {
    categories: ICategory[];
    unitsOfMeasure: IUnitOfMeasure[];
    elements: IElement[];
    composites: IComposite[];
    prices: IPrice[];
    purchases: IElementPurchasedEvent[];
    sales: ICompositeSoldEvent[];
    excerpts: IExcerpt[];
}

function makeId(length) {
    let result = '';
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    const charactersLength = characters.length;
    let counter = 0;
    while (counter < length) {
      result += characters.charAt(Math.floor(Math.random() * charactersLength));
      counter += 1;
    }
    return result;
}

export function mkEntity<T extends IEntityBase>(retn: () => T) {
    const t = retn();
    t.id = makeId(15);

    return t;
}

function mkCategory(name: string, parentCategory?: ICategory | null) {
    return mkEntity<ICategory>(() => {
        return { 
            name: name,
            parentCategoryId: parentCategory?.id
        } as ICategory;
    });
}

function mkUnitOfMeasure(name: string, symbol: string) {
    return mkEntity<IUnitOfMeasure>(() => {
        return { 
            name: name,
            symbol: symbol
        } as IUnitOfMeasure;
    });
}

function mkElement(name: string, category: ICategory, uom: IUnitOfMeasure) {
    return mkEntity<IElement>(() => {
        return {
            name: name,
            categoryId: category.id,
            unitOfMeasureId: uom.id
        } as IElement;
    });
}

function mkComposite(name: string, category: ICategory) {
    return mkEntity<IComposite>(() => {
        return {
            name: name,
            categoryId: category.id
        } as IComposite;
    });
}

function mkPrice(value: number, composite: IComposite) {
    return mkEntity<IPrice>(() => {
        return {
            value: value,
            compositeId: composite.id
        } as IPrice;
    });
}

function mkElementPurchase(element: IElement, numOfUnits: number, unitPrice: number) {
    return mkEntity<IElementPurchasedEvent>(() => {
        return {
            elementId: element.id,
            numOfUnits: numOfUnits,
            unitPrice: unitPrice
        } as IElementPurchasedEvent;
    });
}

function mkExcerpts(composite: IComposite, ingredients: { el: IElement, qty: number }[]) {
    return ingredients.map(ing => {
        return mkEntity<IExcerpt>(() => {
            return {
                compositeId: composite.id,
                elementId: ing.el.id,
                quantity: ing.qty
            } as IExcerpt;
        })
    });
}

export function seedDbMock() {
    // categories
    const bar = mkCategory('Bar');
    const spirits = mkCategory('Spirits', bar);
    const ales = mkCategory('Ales', bar);
    const waters = mkCategory('Waters', bar);
    const veggies = mkCategory('Veggies', bar);
    const cocktails = mkCategory('Cocktails', bar);
    const tikiCocktails = mkCategory('Tiki', cocktails);

    const categories = [bar, spirits, ales, waters, veggies, cocktails, tikiCocktails];

    // units of measure
    const liter = mkUnitOfMeasure('Liter', 'l');
    const kg = mkUnitOfMeasure('Kilogram', 'kg');

    const uoms = [liter, kg];

    // elements
    const vodka = mkElement('vodka', spirits, liter);
    const rum = mkElement('rum', spirits, liter);
    const gin = mkElement('gin', spirits, liter);
    const gingerAle = mkElement('ginger ale', ales, liter);
    const sparklingWater = mkElement('sparkling water', waters, liter);
    const lime = mkElement('lime', veggies, kg);

    const elements = [vodka, rum, gin, gingerAle, sparklingWater, lime];

    // composites
    const moscowMule = mkComposite('moscow mule', cocktails);
    const darkNstormy = mkComposite('dark n stormy ', cocktails);
    const ginFizz = mkComposite('gin fizz', cocktails);

    const composites = [moscowMule, darkNstormy, ginFizz];

    // prices
    const p1 = mkPrice(6, moscowMule);
    const p2 = mkPrice(7, darkNstormy);
    const p3 = mkPrice(4, ginFizz);

    const prices = [p1, p2, p3];

    // purchases
    const pur1 = mkElementPurchase(vodka, 1, 10);
    const pur2 = mkElementPurchase(rum, 1, 20);
    const pur3 = mkElementPurchase(gin, 1, 5);
    const pur4 = mkElementPurchase(gingerAle, 1, 15);
    const pur5 = mkElementPurchase(sparklingWater, 1, 2);
    const pur6 = mkElementPurchase(lime, .5, 5);
    
    const purchases = [pur1, pur2, pur3, pur4, pur5, pur6];

    // excerpts
    const excerpts = [
        {
            composite: moscowMule,
            ingredients: [
                { el: vodka, qty: 0.005 },
                { el: gingerAle, qty: 0.003 },
                { el: lime, qty: 0.025 }
            ]
        },
        {
            composite: darkNstormy,
            ingredients: [
                { el: rum, qty: 0.005 },
                { el: gingerAle, qty: 0.003 },
                { el: lime, qty: 0.025 }
            ]
        },
        {
            composite: ginFizz,
            ingredients: [
                { el: gin, qty: 0.005 },
                { el: sparklingWater, qty: 0.005 },
                { el: lime, qty: 0.025 }
            ]
        }
    ].map(x => mkExcerpts(x.composite, x.ingredients))
    .flat();

    const db: IDatabase = {
        categories: categories,
        unitsOfMeasure: uoms,
        elements: elements,
        composites: composites,
        prices: prices,
        purchases: purchases,
        sales: [],
        excerpts: excerpts
    };

    return db;
}

export async function seedDbAsync() {
    const pb = new PocketBase('http://127.0.0.1:8090');
    const dbMock = seedDbMock();

    const authData = await pb.admins.authWithPassword('admin@admin.com', 'adminadmin');

    await seedCollection('categories', dbMock.categories, pb);
    await seedCollection('unitsOfMeasure', dbMock.unitsOfMeasure, pb);
    await seedCollection('elements', dbMock.elements, pb);
    await seedCollection('composites', dbMock.composites, pb);
    await seedCollection('prices', dbMock.prices, pb);
    await seedCollection('elementPurchasedEvents', dbMock.purchases, pb);
    await seedCollection('excerpts', dbMock.excerpts, pb);

    pb.authStore.clear();
}

async function seedCollection(collectionName: string, items: any[], pb: PocketBase) {
    for(let item of items) {
        try {
            console.log(`seeding: ${collectionName}/${JSON.stringify(item)}`)
            await pb.collection(collectionName).create(item);
        }
        catch(error) {
            let itemJson = JSON.stringify(item, null, 2);
            console.log(`failed to create item in collection ${collectionName}, item: ${itemJson}`);
            console.log(`Error: ${error}`);
        }
    }
}