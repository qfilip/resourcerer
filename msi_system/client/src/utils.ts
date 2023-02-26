import type ICategory from "./interfaces/ICategory";
import type IComposite from "./interfaces/IComposite";
import type ICompositeSoldEvent from "./interfaces/ICompositeSoldEvent";
import type IElement from "./interfaces/IElement";
import type IElementPurchasedEvent from "./interfaces/IElementPurchasedEvent";
import type IEntityBase from "./interfaces/IEntityBase";
import type IExcerpt from "./interfaces/IExcerpt";
import type IPrice from "./interfaces/IPrice";
import type IUnitOfMeasure from "./interfaces/IUnitOfMeasure";

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

export function mkEntity<T extends IEntityBase>(retn: () => T) {
    const now = new Date();
    const t = retn();

    t.id = Math.random().toString(16).substring(2);
    t.createdAt = now;
    t.modifiedAt = now;

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

    const categories = [bar, spirits, ales, waters, veggies, cocktails];

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