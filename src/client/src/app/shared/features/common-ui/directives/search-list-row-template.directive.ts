import { Directive, Input, input } from "@angular/core";
import { SearchListRowTemplateContext } from "../interfaces/search-list-row-template-context.interface";

@Directive({
    selector: 'ng-template[listRow]'
})
export class SearchListRowTemplateDirective<T> {
    @Input('listRow') data!: T[];

    static ngTemplateContextGuard<TContextItem>(
        dir: SearchListRowTemplateDirective<TContextItem>,
        ctx: unknown): ctx is SearchListRowTemplateContext<TContextItem> {
            return true;
        }
}