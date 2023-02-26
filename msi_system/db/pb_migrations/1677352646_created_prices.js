migrate((db) => {
  const collection = new Collection({
    "id": "dmjkfta92veuai8",
    "created": "2023-02-25 19:17:26.994Z",
    "updated": "2023-02-25 19:17:26.994Z",
    "name": "prices",
    "type": "base",
    "system": false,
    "schema": [
      {
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
      },
      {
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
      },
      {
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
      },
      {
        "system": false,
        "id": "x5323i9t",
        "name": "value",
        "type": "number",
        "required": false,
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
  const collection = dao.findCollectionByNameOrId("dmjkfta92veuai8");

  return dao.deleteCollection(collection);
})
