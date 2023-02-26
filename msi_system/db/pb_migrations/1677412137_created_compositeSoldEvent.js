migrate((db) => {
  const collection = new Collection({
    "id": "8f7yus10tv383k4",
    "created": "2023-02-26 11:48:57.377Z",
    "updated": "2023-02-26 11:48:57.377Z",
    "name": "compositeSoldEvent",
    "type": "base",
    "system": false,
    "schema": [
      {
        "system": false,
        "id": "amhzp1i3",
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
      },
      {
        "system": false,
        "id": "k2grimrb",
        "name": "priceId",
        "type": "relation",
        "required": true,
        "unique": false,
        "options": {
          "collectionId": "dmjkfta92veuai8",
          "cascadeDelete": false,
          "maxSelect": 1,
          "displayFields": []
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
  const collection = dao.findCollectionByNameOrId("8f7yus10tv383k4");

  return dao.deleteCollection(collection);
})
