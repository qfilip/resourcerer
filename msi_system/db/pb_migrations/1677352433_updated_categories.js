migrate((db) => {
  const dao = new Dao(db)
  const collection = dao.findCollectionByNameOrId("kijlydfk1vcr6ke")

  // add
  collection.schema.addField(new SchemaField({
    "system": false,
    "id": "qndfefmd",
    "name": "parentCategoryId",
    "type": "relation",
    "required": false,
    "unique": false,
    "options": {
      "collectionId": "kijlydfk1vcr6ke",
      "cascadeDelete": false,
      "maxSelect": 1,
      "displayFields": []
    }
  }))

  return dao.saveCollection(collection)
}, (db) => {
  const dao = new Dao(db)
  const collection = dao.findCollectionByNameOrId("kijlydfk1vcr6ke")

  // remove
  collection.schema.removeField("qndfefmd")

  return dao.saveCollection(collection)
})
