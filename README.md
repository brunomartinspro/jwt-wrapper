## Installation
Package is avaliable via [NuGet](https://nuget.org/packages/JwtWrapper).

## Based on
[Jwt.Net, a JWT (JSON Web Token) implementation for .NET](https://github.com/jwt-dotnet/jwt).

## Usage
### Encoding/Decoding token

```csharp
//Initialize wrapper
IJwtWrapper jwtWrapper = new JwtWrapper();

//Define variables
string secret = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";
var objectToEncode = new StatusDto
{
    Message = "Space Dust",
    Code = HttpStatusCode.OK
};

// Encode object
var tokenOutput = jwtWrapper.Encode(secret, objectToEncode);

//Decode object
var decodeOutput = jwtWrapper.Decode<StatusDto>(secret, tokenOutput?.Content);
```

### Encoding object with default expiration date of one hour

```csharp
// Encode object
var tokenOutput = jwtWrapper.Encode(secret, objectToEncode);
```

### Encoding object with custom expiration date

```csharp
//Set the token expiration day to tomorrow
DateTime tomorrow = DateTime.Now.AddDays(1);
            
// Encode object
var tokenOutput = jwtWrapper.Encode(secret, objectToEncode, tomorrow);
```

### Encoding object with custom algorithm

```csharp
//Set the token expiration day to tomorrow
DateTime tomorrow = DateTime.Now.AddDays(1);

//Create algorithm
var algorithm = new HMACSHA256Algorithm();

// Encode object
var tokenOutput = jwtWrapper.Encode(secret, objectToEncode, tomorrow, algorithm);
```

### Decoding token with json output

```csharp

//Decode object
var decodeOutput = jwtWrapper.Decode(secret, token);

```
### Decoding token with generic object output

```csharp

//Decode object
var decodeOutput = jwtWrapper.Decode<StatusDto>(secret, token);

```

### Common Output Rules

- Code derives from HttpStatusCode;
- Message is a user friendly output;
- Exception provides the full exception with Exception.ToString();
- ExpireDate is in UTC.

### Encode Output Structure

```json
{  
   "Content":"eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJleHAiOjE1NDI1ODcwMDIsImNvbnRlbnQiOnsiTWVzc2FnZSI6IlNwYWNlIER1c3QiLCJFeGNlcHRpb24iOm51bGwsIkNvZGUiOjIwMH19.XIspg2C2SEt9j_tWlar6-OMwJLddHkeaU4UwNhL5dAI",
   "Status":{  
      "Message":"OK",
      "Exception":null,
      "Code":200
   },
   "ExpireDate":"2018-11-19T00:23:22.733"
}

```
- Content is the token;

### Decode Output Structure with Json

```json
{  
   "Content":"{\r\n  \"Message\": \"Space Dust\",\r\n  \"Exception\": null,\r\n  \"Code\": 200\r\n}",
   "Status":{  
      "Message":"OK",
      "Exception":null,
      "Code":200
   },
   "ExpireDate":"2018-11-19T00:32:38.000"
}

```
- Content is the json representation of the object encoded;

### Decode Output Structure with generic object

```json
{  
   "Content":{  
      "Message":"Space Dust",
      "Exception":null,
      "Code":200
   },
   "Status":{  
      "Message":"OK",
      "Exception":null,
      "Code":200
   },
   "ExpireDate":"2018-11-19T00:35:50.000"
}

```
- Content is the object representation of the generic object encoded;
