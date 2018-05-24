
# Probable Enigma - Cache Server

This is a basic cache server that allows you to store anything in memory.
The only requirement is HTTP communication.

## Url Template

	http://localhost:57786/{store}/{key}

Stores represent different collection of items.
Key is the unique id of item.
The following operation are supported:

 - POST stores a new item.
   It should result in HTTP 201 if successful 
   but HTTP 409 if item already exists.
 
 - GET retreives an existing item.
   Its hould result in HTTP 200 if exists
   but HTTP 404 if it doesn't exist.
    
 - PUT replaces an item or adds a new item.
   It should result in HTTP 201 if a new item was added or
   HTTP 202 if an item was replaced.

 - DELETE removes an item from the store.
   If item exists then status code 202 will be returned,
   otherwisse status code 204 will be returned.

Any content type works! The data is stored in memory along with content type.
This means you can cache anything from text to images.

Here is an example of a Javascript frontend using the cache:

	await fetch('strings/greet', { method:'PUT', body:'hello' });
	await fetch('strings/greet').then(async data => console.log(await data.text()));