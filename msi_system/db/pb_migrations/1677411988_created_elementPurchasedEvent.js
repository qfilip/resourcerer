migrate((db) => {
  const collection = new Collection({
    "id": "zgfhj7eiweeqpgu",
    "created": "2023-02-26 11:46:28.719Z",
    "updated": "2023-02-26 11:46:28.719Z",
    "name": "elementPurchasedEvent",
    "type": "base",
    "system": false,
    "schema": [
      {
        "system": false,
        "id": "5vgvqynf",
        "name": "elementId",
        "type": "relation",
        "required": true,
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
        "id": "cd6dipdl",
        "name": "numOfUnits",
        "type": "number",
        "required": true,
        "unique": false,
        "options": {
          "min": null,
          "max": null
        }
      },
      {
        "system": false,
        "id": "hwafpfqu",
        "name": "unitPrice",
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
  const collection = dao.findCollectionByNameOrId("zgfhj7eiweeqpgu");

  return dao.deleteCollection(collection);
})
