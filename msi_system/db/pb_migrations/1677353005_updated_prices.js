migrate((db) => {
  const dao = new Dao(db)
  const collection = dao.findCollectionByNameOrId("dmjkfta92veuai8")

  // add
  collection.schema.addField(new SchemaField({
    "system": false,
    "id": "7jox6ymd",
    "name": "entityStatus",
    "type": "number",
    "required": false,
    "unique": false,
    "options": {
      "min": null,
      "max": null
    }
  }))

  return dao.saveCollection(collection)
}, (db) => {
  const dao = new Dao(db)
  const collection = dao.findCollectionByNameOrId("dmjkfta92veuai8")

  // remove
  collection.schema.removeField("7jox6ymd")

  return dao.saveCollection(collection)
})
