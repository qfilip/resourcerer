import { Directive, Input, input } from "@angular/core";
import { ISearchListRowTemplateContext } from "../interfaces/search-list-row-template-context.interface";

@Directive({
    selector: 'ng-template[typeToken]'
})
export class SearchListRowTemplateDirective<T> {
    @Input('typeToken') typeToken!: T;

    static ngTemplateContextGuard<T>(
        dir: SearchListRowTemplateDirective<T>,
        ctx: unknown): ctx is ISearchListRowTemplateContext<T> {
            return true;
        }
}