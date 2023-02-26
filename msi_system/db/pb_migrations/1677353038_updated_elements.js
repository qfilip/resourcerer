migrate((db) => {
  const dao = new Dao(db)
  const collection = dao.findCollectionByNameOrId("abjb5i6qizx5p4h")

  // add
  collection.schema.addField(new SchemaField({
    "system": false,
    "id": "hwihdeij",
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
  const collection = dao.findCollectionByNameOrId("abjb5i6qizx5p4h")

  // remove
  collection.schema.removeField("hwihdeij")

  return dao.saveCollection(collection)
})
