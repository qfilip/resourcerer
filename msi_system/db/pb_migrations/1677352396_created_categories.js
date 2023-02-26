migrate((db) => {
  const collection = new Collection({
    "id": "kijlydfk1vcr6ke",
    "created": "2023-02-25 19:13:16.505Z",
    "updated": "2023-02-25 19:13:16.505Z",
    "name": "categories",
    "type": "base",
    "system": false,
    "schema": [
      {
        "system": false,
        "id": "jqevjeob",
        "name": "name",
        "type": "text",
        "required": true,
        "unique": true,
        "options": {
          "min": 2,
          "max": null,
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
  const collection = dao.findCollectionByNameOrId("kijlydfk1vcr6ke");

  return dao.deleteCollection(collection);
})
