//     This code was generated by a Reinforced.Typings tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.

export interface ICategoryDto
{
	name: string;
	parentCategoryId: string;
	parentCategory: ICategoryDto;
	childCategories: ICategoryDto[];
	elements: IItemDto[];
}
export interface ICreateCategoryDto
{
	name: string;
	parentCategoryId: string;
}
export interface IExcerptDto
{
	compositeId: string;
	elementId: string;
	element: IItemDto;
	quantity: number;
}
export interface IInstanceDeliveredEventDto
{
	instanceOrderedEventId: string;
	instanceOrderedEvent: IInstanceOrderedEventDto;
}
export interface IInstanceDiscardedEventDto
{
	quantity: number;
	reason: string;
	instanceId: string;
	instance: IInstanceDto;
}
export interface IInstanceDto
{
	quantity: number;
	expiryDate: Date;
	elementId: string;
	element: IItemDto;
	instanceOrderedEvents: IInstanceOrderedEventDto[];
	instanceOrderCancelledEvents: IInstanceOrderCancelledEventDto[];
	instanceDeliveredEvents: IInstanceDeliveredEventDto[];
	instanceDiscardedEvents: IInstanceDiscardedEventDto[];
}
export interface IInstanceInfoDto
{
	instanceId: string;
	quantityLeft: number;
	discards: IDiscardInfoDto[];
	expiryDate: Date;
	purchaseCost: number;
	sellProfit: number;
}
export interface IInstanceOrderCancelledEventDto
{
	instanceOrderedEventId: string;
}
export interface IInstanceOrderedEventDto
{
	itemId: string;
	expiryDate: Date;
	unitsOrdered: number;
	unitPrice: number;
	totalDiscountPercent: number;
	expectedDeliveryDate: Date;
}
export interface ICreateCompositeItemDto
{
	name: string;
	preparationTimeSeconds: number;
	expirationTimeSeconds: number;
	categoryId: string;
	unitOfMeasureId: string;
	unitPrice: number;
	excerptMap: { [key:any]: number };
}
export interface ICreateElementItemDto
{
	name: string;
	preparationTimeSeconds: number;
	expirationTimeSeconds: number;
	categoryId: string;
	unitOfMeasureId: string;
	unitPrice: number;
}
export interface IItemDto
{
	name: string;
	categoryId: string;
	category: ICategoryDto;
	unitOfMeasureId: string;
	unitOfMeasure: IUnitOfMeasureDto;
	excerpts: IExcerptDto[];
	prices: any[];
}
export interface IChangePriceDto
{
	itemId: string;
	unitPrice: number;
}
export interface IPriceDto
{
	unitValue: number;
	elementId: string;
	element: IItemDto;
}
export interface IUnitOfMeasureDto
{
	name: string;
	symbol: string;
	excerpts: IExcerptDto[];
	elements: IItemDto[];
}
export interface IAppUserDto
{
	name: string;
	password: string;
	claims: any[];
}
export interface ISetUserPermissionsDto
{
	userId: string;
	permissions: { [key:string]: string };
}
export interface IItemStockInfoDto
{
	id: string;
	name: string;
	totalUnitsInStock: number;
	instancesInfo: IInstanceInfoDto[];
	itemType: string[];
	productionCostAsComposite: number;
	sellingPrice: number;
}
export interface IItemStatisticsDto
{
	elementId: string;
	name: string;
	unit: string;
	unitsPurchased: number;
	purchaseCosts: number;
	averagePurchaseDiscount: number;
	unitsSold: number;
	salesEarning: number;
	averageSaleDiscount: number;
	unitsUsedInComposites: number;
	usedInComposites: number;
	unitsInStock: number;
}
export interface IDiscardInfoDto
{
	reason: string;
	quantity: number;
}
