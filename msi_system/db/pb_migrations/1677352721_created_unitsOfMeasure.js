migrate((db) => {
  const collection = new Collection({
    "id": "4ma2q1rp87pl2tq",
    "created": "2023-02-25 19:18:41.259Z",
    "updated": "2023-02-25 19:18:41.259Z",
    "name": "unitsOfMeasure",
    "type": "base",
    "system": false,
    "schema": [
      {
        "system": false,
        "id": "ziba3cwk",
        "name": "name",
        "type": "text",
        "required": true,
        "unique": true,
        "options": {
          "min": 2,
          "max": null,
          "pattern": ""
        }
      },
      {
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
      }
    ],
    "listRule": null,
    "viewRule": null,
    "createRule": null,
    "updateRule": null,
    "deleteRule": null,
    "options": {}
  });

  return Dao(db).saveCollection(collection);
}, (db) => {
  const dao = new Dao(db);
  const collection = dao.findCollectionByNameOrId("4ma2q1rp87pl2tq");

  return dao.deleteCollection(collection);
})
