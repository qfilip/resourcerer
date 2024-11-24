<style>
    section {
        border: 2px solid grey;
        padding: .5rem
    }
</style>

# Ideas to implement

<section>

## Recipe versioning

### Title
Implement Recipe versioning.

### Reason
Currently, `CompositeItem` only have `RecipeExcerpt[]`. If composite elements are changed (added, removed or changed in quantity), the history of change is lost.

### Task
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