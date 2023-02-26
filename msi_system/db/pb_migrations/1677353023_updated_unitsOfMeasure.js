migrate((db) => {
  const dao = new Dao(db)
  const collection = dao.findCollectionByNameOrId("4ma2q1rp87pl2tq")

  // add
  collection.schema.addField(new SchemaField({
    "system": false,
    "id": "tcjrwq0g",
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
  const collection = dao.findCollectionByNameOrId("4ma2q1rp87pl2tq")

  // remove
  collection.schema.removeField("tcjrwq0g")

  return dao.saveCollection(collection)
})
