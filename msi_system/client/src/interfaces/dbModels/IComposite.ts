import type IEntityBase from "./IEntityBase";

export default interface IComposite extends IEntityBase {
    name: string;
    categoryId: string;
}