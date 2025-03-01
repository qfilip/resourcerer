<style>
    .pending {
        --color: green;
    }
    .done {
        --color: grey;
    }
    section {
        border-color: var(--color);
        border-style: solid;
        border-width: 2px;
        padding: .5rem;
        margin-bottom: .5rem;
    }
</style>

# Ideas to implement

<section class="pending">

## Recipe table utilization

### Reason
Set recipe version in `CreateCompositeItemProductionOrder` to enable proper tracking of element items usage for producing composite ones.

### Tasks
- Add an endpoint, handler and tests to update composite's recipe.
- Add `RecipeId` in `CreateCompositeItemProductionOrder`
</section>

<section class="done">

## Set event entities to be readonly

### Reason
Events shouldn't be deletable.

### Tasks
Remove `ISoftDeletable` interface from:
- ItemProductionOrder
- InstanceDiscardedEvent
- InstanceOrderedEvent
- InstanceReservedEvent

## Recipe versioning

### Reason
Currently, `CompositeItem` only have `RecipeExcerpt[]`. If composite elements are changed (added, removed or changed in quantity), the history of change is lost.

### Tasks
- Add `Recipe` table.
- Remove `RecipeExcerpt.CompositeId` property.

```json
CompositeItem 1->n Recipe

Recipe = {
    Id: Guid,
    CompositeId: Guid,
    RecipeExcerpts: RecipeExcerpt[],
    Version: int
}

RecipeExcerpt = {
    // composite key
    ElementItemId: Guid,
    RecipeId: Guid,
    // composite key end
    Quantity: decimal
}

Item = {
    Recipes: Recipe[]
}
```
</section>