migrate((db) => {
  const collection = new Collection({
    "id": "oos0kqt0iztsukm",
    "created": "2023-02-25 19:28:36.789Z",
    "updated": "2023-02-25 19:28:36.789Z",
    "name": "excerpts",
    "type": "base",
    "system": false,
    "schema": [
      {
        "system": false,
        "id": "eawjp6ab",
        "name": "compositeId",
        "type": "relation",
        "required": false,
        "unique": false,
        "options": {
          "collectionId": "jvo3ox35ml9k8wi",
          "cascadeDelete": false,
          "maxSelect": 1,
          "displayFields": []
        }
      },
      {
        "system": false,
        "id": "ecuwiqxe",
        "name": "elementId",
        "type": "relation",
        "required": false,
        "unique": false,
        "options": {
          "collectionId": "abjb5i6qizx5p4h",
          "cascadeDelete": false,
          "maxSelect": 1,
          "displayFields": []
        }
      },
      {
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
      },
      {
        "system": false,
        "id": "9eeqotbd",
        "name": "quantity",
        "type": "number",
        "required": true,
        "unique": false,
        "options": {
          "min": null,
          "max": null
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
  const collection = dao.findCollectionByNameOrId("oos0kqt0iztsukm");

  return dao.deleteCollection(collection);
})
