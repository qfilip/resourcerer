migrate((db) => {
  const dao = new Dao(db)
  const collection = dao.findCollectionByNameOrId("dmjkfta92veuai8")

  // remove
  collection.schema.removeField("xe3irbkp")

  // remove
  collection.schema.removeField("zz3yzutz")

  // remove
  collection.schema.removeField("klvq1oxu")

  return dao.saveCollection(collection)
}, (db) => {
  const dao = new Dao(db)
  const collection = dao.findCollectionByNameOrId("dmjkfta92veuai8")

  // add
  collection.schema.addField(new SchemaField({
    "system": false,
    "id": "xe3irbkp",
    "name": "name",
    "type": "text",
    "required": true,
    "unique": true,
    "options": {
      "min": 2,
      "max": null,
      "pattern": ""
    }
  }))

  // add
  collection.schema.addField(new SchemaField({
    "system": false,
    "id": "zz3yzutz",
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

  // add
  collection.schema.addField(new SchemaField({
    "system": false,
    "id": "klvq1oxu",
    "name": "validFrom",
    "type": "date",
    "required": true,
    "unique": false,
    "options": {
      "min": "",
      "max": ""
    }
  }))

  return dao.saveCollection(collection)
})
