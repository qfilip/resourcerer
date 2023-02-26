migrate((db) => {
  const dao = new Dao(db)
  const collection = dao.findCollectionByNameOrId("abjb5i6qizx5p4h")

  // update
  collection.schema.addField(new SchemaField({
    "system": false,
    "id": "wvsyhs2i",
    "name": "priceId",
    "type": "relation",
    "required": false,
    "unique": false,
    "options": {
      "collectionId": "dmjkfta92veuai8",
      "cascadeDelete": false,
      "maxSelect": 1,
      "displayFields": []
    }
  }))

  return dao.saveCollection(collection)
}, (db) => {
  const dao = new Dao(db)
  const collection = dao.findCollectionByNameOrId("abjb5i6qizx5p4h")

  // update
  collection.schema.addField(new SchemaField({
    "system": false,
    "id": "wvsyhs2i",
    "name": "currentPriceId",
    "type": "relation",
    "required": false,
    "unique": false,
    "options": {
      "collectionId": "dmjkfta92veuai8",
      "cascadeDelete": false,
      "maxSelect": 1,
      "displayFields": []
    }
  }))

  return dao.saveCollection(collection)
})
