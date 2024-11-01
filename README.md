# AuthenticationProvider
Hantera autentisering av användare. Detta kan inkludera traditionella inloggningar, OAuth-integrationer (t.ex. Google, Facebook) och sessionshantering.


## ENDPOINTS:

### POST:
#### `https://rika-authenticationprovider-drfta9bhdaf0g0dr.westeurope-01.azurewebsites.net/api/Auth/signup`
#### `https://rika-authenticationprovider-drfta9bhdaf0g0dr.westeurope-01.azurewebsites.net/api/Auth/signin`


### GET:
#### `https://rika-authenticationprovider-drfta9bhdaf0g0dr.westeurope-01.azurewebsites.net/api/User/getself`
#### `https://rika-authenticationprovider-drfta9bhdaf0g0dr.westeurope-01.azurewebsites.net/api/User/getallusers`
#### `https://rika-authenticationprovider-drfta9bhdaf0g0dr.westeurope-01.azurewebsites.net/api/User/getbyid/"UserId"`
#### `https://rika-authenticationprovider-drfta9bhdaf0g0dr.westeurope-01.azurewebsites.net/api/User/getbyemail/"email"`


### PUT:
#### `https://rika-authenticationprovider-drfta9bhdaf0g0dr.westeurope-01.azurewebsites.net/api/User/updatebyid/"UserId"`


### DELETE:
#### `https://rika-authenticationprovider-drfta9bhdaf0g0dr.westeurope-01.azurewebsites.net/api/User/deletebyid/"UserId"`


## Models:

SignUpModel:
```
public class SignUpModel
{
    public string Email { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool IsAdmin { get; set; } = false; // ändra ENDAST detta på adminplatformen, modellen i webappen för kunder behöver INTE skicka med IsAdmin för den är automatiskt inställd till FALSE.
}
```
**Svar:** 200 OK


SignInModel:
```
public class SignInModel
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
```
**Svar:** 200 OK och:
``
{
    "token": "Din-personliga-token"
}
``

## DTOs:

UserDto
```
public class UserDto
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsAdmin { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? ProfileDescription { get; set; }
    public string? Gender { get; set; }
    public int? Age { get; set; }
}
```
