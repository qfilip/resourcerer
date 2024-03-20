# Todo

- CreateItemProductionOrder
    - validate data integrity
    - can only produce owned item with owned instances
    - use `Item.Category` for `CompanyId` validation

- Instances
    - must be mapped to `Item` of owner company. Clone if it doesn't exist, or map to specified `Item`
    - move json event data to relational entity (`OrderedEvents`)
    - move json event data to relational entity (`DiscardedEvents`)
    - move json event data to relational entity (`ReservedEvents`)
