# AuthenticationProvider
Hantera autentisering av användare. Detta kan inkludera traditionella inloggningar, OAuth-integrationer (t.ex. Google, Facebook) och sessionshantering.


## ENDPOINTS:

### /api/Auth/signup

### /api/Auth/signin


## Models:

SignUpModel:
```
public class SignUpModel
{
    public string Email { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool IsAdmin { get; set; } = false; // ändra **ENDAST** detta på adminplatformen, modellen i webappen för kunder behöver **INTE** skicka med IsAdmin för den är automatiskt inställd till FALSE.
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
