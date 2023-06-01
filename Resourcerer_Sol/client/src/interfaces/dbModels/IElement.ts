import type IEntityBase from "./IEntityBase";

export default interface IElement extends IEntityBase {
    name: string;
    categoryId: string;
    unitOfMeasureId: string;
}