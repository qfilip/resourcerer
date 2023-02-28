import type IEntityBase from "../dbModels/IEntityBase";

export default interface IElementTableDetails extends IEntityBase  {
    name: string;
    category: string;
    unitOfMeasure: string;
    inStock: number;
}