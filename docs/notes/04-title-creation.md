# Title Creation

This step adds a POST /titles endpoint for creating catalog entries

we defiend data structure previously, now we receive that(CreateTitileRequest) on post request.
it is deserialized by System.Text.Json automatically, so we only need to confirm wether our receiving data's integrety

(ASP.NET Core automatically deserializes the JSON request body into a CreateTitleRequest using System.Text.Json. The endpoint then validates the required fields before creating the title.)

then we insert input data into dictionary upon validation, and return 201,localtion and data entity

Previously, we were using numbers as id, which is in appropriate(since new id is simply fetching last id and +1), so we changed that into GUID to prevent same ID generation upon multiple concurrent request