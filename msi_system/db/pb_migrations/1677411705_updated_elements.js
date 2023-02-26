migrate((db) => {
  const dao = new Dao(db)
  const collection = dao.findCollectionByNameOrId("abjb5i6qizx5p4h")

  // add
  collection.schema.addField(new SchemaField({
    "system": false,
    "id": "8vld8d0y",
    "name": "unitOfMeasureId",
    "type": "relation",
    "required": true,
    "unique": false,
    "options": {
      "collectionId": "4ma2q1rp87pl2tq",
      "cascadeDelete": false,
      "maxSelect": 1,
      "displayFields": []
    }
  }))

  return dao.saveCollection(collection)
}, (db) => {
  const dao = new Dao(db)
  const collection = dao.findCollectionByNameOrId("abjb5i6qizx5p4h")

  // remove
  collection.schema.removeField("8vld8d0y")

  return dao.saveCollection(collection)
})
