# Diagram

Use [dbdiagram.io](https://dbdiagram.io/home) for editing

```
table Company as cmp {
  Id guid [pk]
  Name string
}

table AppUser as user {
  Id guid [pk]
  Name string

  CompanyId guid [ref:> cmp.Id]
}

table Category as ctg {
  Id guid [pk]
  Name string
  
  ParentCategoryId guid [ref:> ctg.Id]
  CompanyId guid [ref:> cmp.Id]
}

table Excerpt as exc {
  Id guid [pk]
  CompositeId guid [ref:> item.Id]
  ElementId guid [ref:> item.Id]
  Quantity int
}

table UnitOfMeasure as uom {
  Id guid [pk]
  Name string
  Symbol string

  CompanyId guid [ref:> cmp.Id]
}

table Price as prc {
  Id guid [pk]
  Value float

  ItemId guid [ref:> item.Id]
}

table Item as item {
  Id guid [pk]
  Name float

  UnitOfMeasureId guid [ref:> uom.Id]
  CategoryId guid [ref:> ctg.Id]
}

table Recipe as recipe {
  Id guid [pk]
  Version int
  CompositeId guid [ref:> item.Id]
}

table RecipeExcerpt as recipeExcerpt {
  Id guid [pk]
  ElementId guid [ref :> item.Id]
  RecipeId guid [ref:> recipe.Id]
}

table ItemProductionOrder as itmProdOrd {
  Id guid
  Quantity double
  Reason string
  CompanyId guid

  ItemId guid [ref:> item.Id]
}

table Instance as inst {
  Id guid
  Quantity double
  ExpryDate date

  ItemId guid [ref:> item.Id]
  OwnerCompanyId guid [ref:> cmp.Id]
  SourceInstanceId guid [ref:> inst.Id]
}

table InstanceOrderedEvent as instOrdEv {
  SellerCompanyId guid
  BuyerCompanyId guid
  Quantity double
  UnitPrice double
  DiscountPercent double

  Cancelled bool
  Sent bool
  Delivered bool

  InstanceId guid [ref:> inst.Id]
}

table InstanceReservedEvent as instResEv {
  ItemProductionOrderId guid
  Quantity double

  Cancelled bool
  Used bool
  InstanceId guid [ref:> inst.Id]
}

table InstanceDiscardedEvent as instDiscEv {
  Quantity double
  Reason string

  InstanceId guid [ref:> inst.Id]
}
```