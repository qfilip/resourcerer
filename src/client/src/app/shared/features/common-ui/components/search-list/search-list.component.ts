import { CommonModule } from '@angular/common';
import { Component, computed, ContentChild, input, output, signal, TemplateRef } from '@angular/core';
import { SearchListRowTemplateDirective } from '../../directives/search-list-row-template.directive';

@Component({
  selector: 'app-search-list',
  imports: [CommonModule],
  templateUrl: './search-list.component.html',
  styleUrl: './search-list.component.css'
})
export class SearchListComponent<T> {

  @ContentChild(SearchListRowTemplateDirective, {read: TemplateRef }) rows?: TemplateRef<any>;
  $data = input.required<T[]>();
  $selected = input.required<T | null>();
  $displayFilter = input.required<(query: string) => (x: T) => boolean>();
  private $query = signal<string>('');

  onSelected = output<T>();

  $displayed = computed(() => {
    const items = this.$data();
    const query = this.$query().toLowerCase();
    const filterFn = this.$displayFilter()(query);

    return items.filter(x => filterFn(x));
  });

  onQueryChanged = (query: string) => this.$query.set(query);
}
