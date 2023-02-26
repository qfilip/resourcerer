migrate((db) => {
  const collection = new Collection({
    "id": "abjb5i6qizx5p4h",
    "created": "2023-02-25 19:21:18.616Z",
    "updated": "2023-02-25 19:21:18.616Z",
    "name": "elements",
    "type": "base",
    "system": false,
    "schema": [
      {
        "system": false,
        "id": "wtfozcij",
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
        "id": "rmbkgwnp",
        "name": "categoryId",
        "type": "relation",
        "required": false,
        "unique": false,
        "options": {
          "collectionId": "kijlydfk1vcr6ke",
          "cascadeDelete": false,
          "maxSelect": 1,
          "displayFields": []
        }
      },
      {
        "system": false,
        "id": "wvsyhs2i",
        "name": "currentPriceId",
        "type": "relation",
        "required": false,
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
  const collection = dao.findCollectionByNameOrId("abjb5i6qizx5p4h");

  return dao.deleteCollection(collection);
})
