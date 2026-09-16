# UserManagementAPI

This project is a peer-reviewed assignment for the Coursera course **Backend Development with .NET**, developed with the help of **GitHub Copilot** to meet the course requirements.

## Debugging Notes

Copilot's review identified missing input validation, unsafe handling of unexpected exceptions, and avoidable list copying in `GET /api/users`. The fixes add required name and email validation, reject non-positive IDs, return an array snapshot for collection reads, and log unexpected failures before returning an RFC 7807 `ProblemDetails` response.

The API uses in-memory storage, so data is reset whenever the application restarts.

## Endpoints

| Method | Route | Success response |
| --- | --- | --- |
| GET | `/api/users` | `200 OK` with all users |
| GET | `/api/users/{id}` | `200 OK` or `404 Not Found` |
| POST | `/api/users` | `201 Created` |
| PUT | `/api/users/{id}` | `204 No Content` |
| DELETE | `/api/users/{id}` | `204 No Content` |

Invalid names, invalid email addresses, and malformed requests return `400 Bad Request`. Unexpected server errors return `500 Internal Server Error` with `ProblemDetails`.