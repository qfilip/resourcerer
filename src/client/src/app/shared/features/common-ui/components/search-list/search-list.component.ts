import { Component, computed, input, output, signal } from '@angular/core';

@Component({
  selector: 'app-search-list',
  imports: [],
  templateUrl: './search-list.component.html',
  styleUrl: './search-list.component.css'
})
export class SearchListComponent {
  $data = input.required<any[]>();
  $selected = input.required<any>();
  $displayFilter = input.required<(query: string) => (x: any) => boolean>();
  private $query = signal<string>('');

  onSelected = output<any>();

  $displayed = computed(() => {
    const items = this.$data();
    const query = this.$query().toLowerCase();
    const filterFn = this.$displayFilter()(query);

    return items.filter(x => filterFn(x));
  });

  onQueryChanged = (query: string) => this.$query.set(query);
}
