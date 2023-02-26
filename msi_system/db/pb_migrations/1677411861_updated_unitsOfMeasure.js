migrate((db) => {
  const dao = new Dao(db)
  const collection = dao.findCollectionByNameOrId("4ma2q1rp87pl2tq")

  // update
  collection.schema.addField(new SchemaField({
    "system": false,
    "id": "kmc1pqv2",
    "name": "symbol",
    "type": "text",
    "required": true,
    "unique": false,
    "options": {
      "min": 2,
      "max": 7,
      "pattern": ""
    }
  }))

  return dao.saveCollection(collection)
}, (db) => {
  const dao = new Dao(db)
  const collection = dao.findCollectionByNameOrId("4ma2q1rp87pl2tq")

  // update
  collection.schema.addField(new SchemaField({
    "system": false,
    "id": "kmc1pqv2",
    "name": "abbreviation",
    "type": "text",
    "required": true,
    "unique": false,
    "options": {
      "min": 2,
      "max": 4,
      "pattern": ""
    }
  }))

  return dao.saveCollection(collection)
})
