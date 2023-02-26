migrate((db) => {
  const dao = new Dao(db)
  const collection = dao.findCollectionByNameOrId("dmjkfta92veuai8")

  // add
  collection.schema.addField(new SchemaField({
    "system": false,
    "id": "vzofbppq",
    "name": "compositeId",
    "type": "relation",
    "required": true,
    "unique": false,
    "options": {
      "collectionId": "jvo3ox35ml9k8wi",
      "cascadeDelete": false,
      "maxSelect": 1,
      "displayFields": []
    }
  }))

  return dao.saveCollection(collection)
}, (db) => {
  const dao = new Dao(db)
  const collection = dao.findCollectionByNameOrId("dmjkfta92veuai8")

  // remove
  collection.schema.removeField("vzofbppq")

  return dao.saveCollection(collection)
})
