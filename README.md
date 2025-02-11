# Thavyra

This project contains the Thavyra Open ID Connect authorization server and REST API.

## OpenID Connect

### Claims

The following claims are provided by identity tokens and the userinfo endpoint.

| Name                 | Description                                                                                                                                                                                                      | Example                                                                       |
|----------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-------------------------------------------------------------------------------|
| `sub`                | User ID                                                                                                                                                                                                          | 06ae10e4-1376-4cb0-8dbb-f8ee3d125fad                                          |
| `email`              | Thavyra does not collect user emails however services often require OpenID providers to provide an email claim. A valid email address is provided for each user but it should not be expected to be functional.  | john@users.thavyra.xyz                                                        |
| `name`               | The user's username, as set on the dashboard.                                                                                                                                                                    | john                                                                          |
| `nickname`           | The user's username. Reserved for possible future nickname functionality.                                                                                                                                        | john                                                                          |
| `preferred_username` | The user's username. Reserved for possible future use.                                                                                                                                                           | john                                                                          |
| `profile`            | URL of the user's profile page.                                                                                                                                                                                  | https://thavyra.xyz/@john                                                     |
| `picture`            | URL of the user's avatar.                                                                                                                                                                                        | https://thavyra.xyz/api/users/06ae10e4-1376-4cb0-8dbb-f8ee3d125fad/avatar.png |



## First Time Setup

Register an account to complete initial system setup. 
This account will be given the admin role and assigned the owner of the system application. 
The Client ID and Client Secret will be written to the log.
Grant the scp:applications permission to register the dashboard.

## API Docs

Redoc documentation for the REST api can be found at /docs/rest
