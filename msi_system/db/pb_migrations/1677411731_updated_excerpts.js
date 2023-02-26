migrate((db) => {
  const dao = new Dao(db)
  const collection = dao.findCollectionByNameOrId("oos0kqt0iztsukm")

  // remove
  collection.schema.removeField("vwt7bemz")

  return dao.saveCollection(collection)
}, (db) => {
  const dao = new Dao(db)
  const collection = dao.findCollectionByNameOrId("oos0kqt0iztsukm")

  // add
  collection.schema.addField(new SchemaField({
    "system": false,
    "id": "vwt7bemz",
    "name": "unitOfMeasureId",
    "type": "relation",
    "required": false,
    "unique": false,
    "options": {
      "collectionId": "4ma2q1rp87pl2tq",
      "cascadeDelete": false,
      "maxSelect": 1,
      "displayFields": []
    }
  }))

  return dao.saveCollection(collection)
})
