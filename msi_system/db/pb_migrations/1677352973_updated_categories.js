migrate((db) => {
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

  // add
  collection.schema.addField(new SchemaField({
    "system": false,
    "id": "i1rxfnxm",
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
  const collection = dao.findCollectionByNameOrId("kijlydfk1vcr6ke")

  // remove
  collection.schema.removeField("ajxo0h56")

  // remove
  collection.schema.removeField("i1rxfnxm")

  return dao.saveCollection(collection)
})
