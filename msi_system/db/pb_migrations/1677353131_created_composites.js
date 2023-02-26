migrate((db) => {
  const collection = new Collection({
    "id": "jvo3ox35ml9k8wi",
    "created": "2023-02-25 19:25:31.959Z",
    "updated": "2023-02-25 19:25:31.959Z",
    "name": "composites",
    "type": "base",
    "system": false,
    "schema": [
      {
        "system": false,
        "id": "0gkdqddn",
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
        "id": "y5kawzcw",
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
  const collection = dao.findCollectionByNameOrId("jvo3ox35ml9k8wi");

  return dao.deleteCollection(collection);
})
