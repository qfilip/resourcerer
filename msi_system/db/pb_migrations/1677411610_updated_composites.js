migrate((db) => {
  const dao = new Dao(db)
  const collection = dao.findCollectionByNameOrId("jvo3ox35ml9k8wi")

  // remove
  collection.schema.removeField("pfwifnqx")

  return dao.saveCollection(collection)
}, (db) => {
  const dao = new Dao(db)
  const collection = dao.findCollectionByNameOrId("jvo3ox35ml9k8wi")

  // add
  collection.schema.addField(new SchemaField({
    "system": false,
    "id": "pfwifnqx",
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
})
