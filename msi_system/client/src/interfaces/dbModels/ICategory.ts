import type IEntityBase from "./IEntityBase";

export default interface ICategory extends IEntityBase {
    name: string;
    parentCategoryId: string;
}