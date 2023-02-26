migrate((db) => {
  const dao = new Dao(db)
  const collection = dao.findCollectionByNameOrId("kijlydfk1vcr6ke")

  // remove
  collection.schema.removeField("ajxo0h56")

  return dao.saveCollection(collection)
}, (db) => {
  const dao = new Dao(db)
  const collection = dao.findCollectionByNameOrId("kijlydfk1vcr6ke")

  // add
  collection.schema.addField(new SchemaField({
    "system": false,
    "id": "ajxo0h56",
    "name": "field",
    "type": "text",
    "required": false,
    "unique": false,
    "options": {
      "min": null,
      "max": null,
      "pattern": ""
    }
  }))

  return dao.saveCollection(collection)
})
