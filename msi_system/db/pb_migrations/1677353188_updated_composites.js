migrate((db) => {
  const dao = new Dao(db)
  const collection = dao.findCollectionByNameOrId("jvo3ox35ml9k8wi")

  // add
  collection.schema.addField(new SchemaField({
    "system": false,
    "id": "gmezdx8k",
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
  const collection = dao.findCollectionByNameOrId("jvo3ox35ml9k8wi")

  // remove
  collection.schema.removeField("gmezdx8k")

  return dao.saveCollection(collection)
})
